namespace ProductManagement.Features.DTOs;

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