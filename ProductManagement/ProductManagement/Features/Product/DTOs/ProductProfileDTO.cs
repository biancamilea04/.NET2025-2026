namespace ProductManagement.Features.Product.DTOs;

public class ProductProfileDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string FormattedPrice { get; init; } = string.Empty;
    public DateTime ReleaseDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? ImageUrl { get; init; }

    public bool IsAvailable { get; init; }
    public int StockQuantity { get; init; }
    public string ProductAge { get; init; } = string.Empty;
    public string BrandInitials { get; init; } = string.Empty;
    public string AvailabilityStatus { get; init; } = string.Empty;
}