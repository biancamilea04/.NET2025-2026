using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManagement.Common.Logging;
using ProductManagement.Common.Mapping;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;
using ProductManagement.Features.Products;
using ProductManagement.Features.Request;
using ProductManagement.Persistence;
using ProductManagement.Validators;

namespace ProductManagement.Test;

public class CreateProductHandlerIntegrationTests : IDisposable
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<CreateProductHandler>> _mockLogger;
    private readonly IValidator<CreateProductProfileRequest> _validator;
    private readonly CreateProductHandler _handler;
    private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;

    public CreateProductHandlerIntegrationTests()
    {
        _connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var dbOptions = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationContext(dbOptions);
        _context.Database.EnsureCreated();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AdvancedProductMappingProfile>();
        }, new LoggerFactory());
        _mapper = mapperConfig.CreateMapper();

        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _mockLogger = new Mock<ILogger<CreateProductHandler>>();

        _validator = new CreateProductProfileValidator(_context, new Mock<ILogger<CreateProductProfileValidator>>().Object);

        _handler = new CreateProductHandler(_mapper, _context, _mockLogger.Object, _validator);
    }

    [Fact]
    public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
    {
        // Arrange: Create valid Electronics product request with all properties
        var releaseDate = DateTime.UtcNow.AddDays(-60);
        var request = new CreateProductProfileRequest(
            Name: "Smart Wireless Bluetooth Headphones",
            Brand: "Samsung Electronics",
            SKU: "SAM12345",
            Category: ProductCategory.Electronics,
            Price: 99.99m,
            ReleaseDate: releaseDate,
            ImageUrl: "https://example.com/samsung.jpg",
            StockQuantity: 15
        );

        // Act: Call handler
        var result = await _handler.Handle(request);

        // Assert: Verify Ok result type with ProductProfileDto payload
        Assert.NotNull(result);
        var okResult = Assert.IsType<Ok<ProductProfileDto>>(result);
        Assert.NotNull(okResult.Value);
        
        var productDto = okResult.Value;

        // Assert: Check product was created with correct name
        Assert.Equal("Smart Wireless Bluetooth Headphones", productDto.Name);
        Assert.Equal("Samsung Electronics", productDto.Brand);
        Assert.Equal("SAM12345", productDto.SKU);

        // Assert: Verify ProductCreationStarted log called once
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.Is<EventId>(e => e.Id == LogEvents.ProductCreationStarted),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "ProductCreationStarted event should be logged exactly once");
    }

    [Fact]
    public async Task Handle_DuplicateSKU_ThrowsValidationExceptionWithLogging()
    {
        // Arrange: Create existing product in database with specific SKU
        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Product",
            Brand = "ExistingBrand",
            SKU = "DUP00001",
            Category = ProductCategory.Electronics,
            Price = 299.99m,
            ReleaseDate = DateTime.UtcNow.AddDays(-30),
            ImageUrl = "https://example.com/existing.jpg",
            StockQuantity = 10,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Products.Add(existingProduct);
        _context.SaveChanges();

        // Arrange: Create request with same SKU
        var duplicateRequest = new CreateProductProfileRequest(
            Name: "New Product",
            Brand: "NewBrand",
            SKU: "DUP00001",
            Category: ProductCategory.Electronics,
            Price: 199.99m,
            ReleaseDate: DateTime.UtcNow,
            ImageUrl: "https://example.com/new.jpg",
            StockQuantity: 20
        );

        // Act & Assert: Verify ValidationException thrown
        var exception = await Assert.ThrowsAsync<ValidationException>(
            async () => await _handler.Handle(duplicateRequest));

        // Assert: Check exception message contains "already exists" or similar validation error
        Assert.NotNull(exception.Errors);
        Assert.NotEmpty(exception.Errors);
        var errorMessages = string.Join(" | ", exception.Errors.Select(e => e.ErrorMessage));
        Assert.Contains("must be unique", errorMessages);

        // Assert: Verify ProductValidationFailed log called once
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.Is<EventId>(e => e.Id == LogEvents.ProductValidationFailed),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "ProductValidationFailed event should be logged exactly once");
    }

    [Fact]
    public async Task Handle_HomeProductRequest_AppliesDiscountAndConditionalMapping()
    {
        // Arrange: Create valid Home product request
        var releaseDate = DateTime.UtcNow.AddDays(-45);
        var originalPrice = 150.00m;
        var request = new CreateProductProfileRequest(
            Name: "Wooden Dining Table",
            Brand: "FurnitureCo",
            SKU: "HOME0001",
            Category: ProductCategory.Home,
            Price: originalPrice,
            ReleaseDate: releaseDate,
            ImageUrl: "https://example.com/table.jpg",
            StockQuantity: 8
        );

        // Act: Call handler
        var result = await _handler.Handle(request);

        // Assert: Verify Ok result type with ProductProfileDto payload
        Assert.NotNull(result);
        var okResult = Assert.IsType<Ok<ProductProfileDto>>(result);
        Assert.NotNull(okResult.Value);
        
        var productDto = okResult.Value;

        // Assert: Check product was created with correct information
        Assert.Equal("Wooden Dining Table", productDto.Name);
        Assert.Equal("FurnitureCo", productDto.Brand);
        Assert.Equal("HOME0001", productDto.SKU);

        // Assert: Check ImageUrl is null (content filtering for Home category)
        Assert.Null(productDto.ImageUrl);
    }

    
    public void Dispose()
    {
        _context?.Dispose();
        _memoryCache?.Dispose();
    }
}
