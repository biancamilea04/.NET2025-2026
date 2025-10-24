namespace ProductManagement.Features.Product;

public class ProductProfileDTO
{
    Guid Id { get; init; }
    string Name { get; init; }
    string Brand { get; init; }
    string SKU { get; init; }
    decimal Price { get; init; }
    string FormattedPrice { get; init; }
    DateTime ReleaseDate { get; init; }
    DateTime CreatedAt { get; init; }
    string? ImageUrl { get; init; }

    bool IsAvailable { get; init; }
    int StockQuantity { get; init; }
    string ProductAge { get; init; }
    string BrandInitials { get; init; }
    string AvailabilityStatus { get; init; }
}