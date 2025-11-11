namespace ProductManagement.Features.Request;

public record CreateProductProfileRequest(
    string Name,
    string Brand,
    string SKU,
    ProductCategory Category,
    decimal Price,
    DateTime ReleaseDate,
    string? ImageUrl = null ,
    int StockQuantity = 1
    );