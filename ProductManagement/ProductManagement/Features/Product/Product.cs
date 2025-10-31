namespace ProductManagement.Features.Product;

public record Product(
    string Name,
    string Description,
    string SKU,
    ProductCategory Category,
    decimal Price,
    DateTime ReleaseDate,
    string? ImageUrl,
    bool IsAvaliable,
    int StockQuantity = 0,
    Guid Id = default,
    DateTime CreatedAt = default,
    DateTime? UpdatedAt = null);
