using ProductManagement.Features.Product.DTOs;
using ProductManagement.Persistence;
using ProductManagement.Validator;

namespace ProductManagement.Features.Product;

using System;
using System.Collections.Concurrent;
using System.Linq;
using ProductManagement.Features.Product.DTOs;

public class CreateProductHandler(ApplicationContext context, ILogger<CreateProductHandler> logger)
{

    public ProductProfileDTO Handle(CreateProductProfileRequest createProductProfileRequest)
    {
        var validator = new CreateProductProfileValidator(logger);
        var validationResult = validator.Validate(createProductProfileRequest);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException("Invalid product profile request: " +
                                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var product = MapToProduct(createProductProfileRequest);

        return MapToDto(product);
    }

    private static Product MapToProduct(CreateProductProfileRequest r)
    {
        var product = new Product(
            Name: r.Name,
            Description: r.Brand ?? string.Empty,
            SKU: r.SKU,
            Category: r.Category,
            Price: r.Price,
            ReleaseDate: r.ReleaseDate,
            ImageUrl: r.ImageUrl,
            IsAvaliable: r.StockQuantity > 0,
            StockQuantity: r.StockQuantity
        );

        return product;
    }

    private static ProductProfileDTO MapToDto(Product p)
    {
        var formattedPrice = p.Price.ToString("C");
        var createdAt = DateTime.UtcNow;
        var productAge = (DateTime.UtcNow - p.ReleaseDate).Days switch
        {
            < 0 => "Not released",
            0 => "New",
            1 => "1 day",
            var d => $"{d} days"
        };

        var nameForInitials = p.Name ?? string.Empty;
        var brandInitials = string
            .Concat(nameForInitials.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x[0]))
            .ToUpperInvariant();
        var availabilityStatus = p.IsAvaliable ? "Available" : "Unavailable";

        return new ProductProfileDTO
        {
            Id = Guid.NewGuid(),
            Name = p.Name,
            Brand = p.Description?.Split(' ').FirstOrDefault() ?? string.Empty,
            SKU = p.SKU,
            Price = p.Price,
            FormattedPrice = formattedPrice,
            ReleaseDate = p.ReleaseDate,
            CreatedAt = createdAt,
            ImageUrl = p.ImageUrl,
            IsAvailable = p.IsAvaliable,
            StockQuantity = p.StockQuantity,
            ProductAge = productAge,
            BrandInitials = brandInitials,
            AvailabilityStatus = availabilityStatus
        };
    }
}