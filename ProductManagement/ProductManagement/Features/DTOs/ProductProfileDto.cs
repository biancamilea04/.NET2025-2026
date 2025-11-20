namespace ProductManagement.Features.DTOs;

/// <summary>
/// Data Transfer Object for product profile information.
/// Represents a product with formatted and computed display properties for API responses.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Brand">The brand or manufacturer of the product.</param>
/// <param name="SKU">The Stock Keeping Unit for inventory tracking.</param>
/// <param name="CategoryDisplayName">Human-readable category name (e.g., "Electronics & Technology").</param>
/// <param name="Price">The original price of the product.</param>
/// <param name="FormattedPrice">The price formatted as a currency string.</param>
/// <param name="ReleaseDate">The date when the product was first released.</param>
/// <param name="CreatedAt">The timestamp when the product was created in the system.</param>
/// <param name="ImageUrl">The URL to the product image, if available.</param>
/// <param name="IsAvailable">Indicates whether the product is currently available for purchase.</param>
/// <param name="StockQuantity">The current quantity in stock.</param>
/// <param name="ProductAge">Human-readable product age (e.g., "2 months old", "New Release", "Classic").</param>
/// <param name="BrandInitials">The initials derived from the brand name.</param>
/// <param name="AvailabilityStatus">Human-readable availability status (e.g., "In Stock", "Limited Stock", "Out of stock").</param>
public record ProductProfileDto(
    Guid Id = default,
    string Name = "",
    string Brand = "",
    string SKU = "",
    string CategoryDisplayName = "",
    decimal Price = 0,
    string FormattedPrice = "",
    DateTime ReleaseDate = default,
    DateTime CreatedAt = default,
    string? ImageUrl = null,
    bool IsAvailable = false,
    int StockQuantity = 0,
    string ProductAge = "",
    string BrandInitials = "",
    string AvailabilityStatus = ""
);