namespace ProductManagement.Features.Request;

/// <summary>
/// Request model for creating a new product in the Product Management system.
/// Encapsulates all required and optional product information for the creation process.
/// </summary>
/// <param name="Name">The name of the product. Must be unique and between 1-200 characters.</param>
/// <param name="Brand">The brand or manufacturer name. Must be between 2-100 characters.</param>
/// <param name="SKU">The Stock Keeping Unit - a unique 8-character alphanumeric identifier for inventory tracking.</param>
/// <param name="Category">The product category classification (Electronics, Clothing, Books, or Home).</param>
/// <param name="Price">The product price in decimal format. Must be between 0.01 and 9,999.99.</param>
/// <param name="ReleaseDate">The date when the product was first released. Used for calculating product age.</param>
/// <param name="ImageUrl">Optional URL pointing to the product image. May be null for certain categories.</param>
/// <param name="StockQuantity">The initial stock quantity. Must be positive and not exceed 100,000. Defaults to 1.</param>
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