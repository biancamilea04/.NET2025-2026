using AutoMapper;
using ProductManagement.Features.Product;
using ProductManagement.Features.Product.DTOs;
using System.Globalization;
using System.Linq;

namespace ProductManagement.Common.Mapping;

public class AdvanceProductMappingProfile : Profile
{
    public AdvanceProductMappingProfile()
    {
        // Map CreateProductProfileRequest -> Product
        CreateMap<CreateProductProfileRequest, Product>()
            .ForCtorParam("Name", opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Brand))
            .ForCtorParam("SKU", opt => opt.MapFrom(src => src.SKU))
            .ForCtorParam("Category", opt => opt.MapFrom(src => src.Category))
            .ForCtorParam("Price", opt => opt.MapFrom(src => src.Price))
            .ForCtorParam("ReleaseDate", opt => opt.MapFrom(src => src.ReleaseDate))
            .ForCtorParam("ImageUrl", opt => opt.MapFrom(src => src.ImageUrl))
            .ForCtorParam("IsAvaliable", opt => opt.MapFrom(src => src.StockQuantity > 0))
            .ForCtorParam("StockQuantity", opt => opt.MapFrom(src => src.StockQuantity))
            .ForCtorParam("Id", opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForCtorParam("UpdatedAt", opt => opt.MapFrom(_ => (DateTime?)null));

        // Map Product -> ProductProfileDTO using inline functions (no resolver classes)
        CreateMap<Product, ProductProfileDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id == Guid.Empty ? Guid.NewGuid() : src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src => FormatPrice(src.Price)))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt == default ? DateTime.UtcNow : src.CreatedAt))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvaliable))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
            .ForMember(dest => dest.ProductAge, opt => opt.MapFrom(src => ResolveProductAge(src)))
            .ForMember(dest => dest.BrandInitials, opt => opt.MapFrom(src => ResolveBrandInitials(src)))
            .ForMember(dest => dest.AvailabilityStatus, opt => opt.MapFrom(src => ResolveAvailabilityStatus(src)))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => GetBrandFromDescriptionOrEmpty(src.Description)))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => ResolveCategoryDisplay(src)));
    }

    private static string FormatPrice(decimal price)
    {
        // Format as currency with two decimals
        return price.ToString("C2", CultureInfo.CurrentCulture);
    }

    private static string ResolveProductAge(Product source)
    {
        var days = (DateTime.UtcNow - source.ReleaseDate).Days;
        if (days < 0) return "Not released";
        if (days < 30) return "New Release";
        if (days == 1825) return "Classic";
        if (days < 365)
        {
            var months = Math.Max(1, days / 30);
            return $"{months} months old";
        }

        if (days < 1825)
        {
            var years = days / 365;
            return $"{years} years old";
        }

        // days > 1825
        var yrs = days / 365;
        return $"{yrs} years old";
    }

    private static string ResolveBrandInitials(Product source)
    {
        var brand = GetBrandFromDescriptionOrEmpty(source.Description);
        if (string.IsNullOrWhiteSpace(brand)) return "?";

        var parts = brand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return parts[0][0].ToString().ToUpperInvariant();
        }
        
        var first = parts[0][0];
        var last = parts[^1][0];
        return string.Concat(char.ToUpperInvariant(first), char.ToUpperInvariant(last));
    }

    private static string ResolveAvailabilityStatus(Product source)
    {
        if (!source.IsAvaliable) return "Out of Stock";

        var qty = source.StockQuantity;
        if (qty == 0) return "Unavailable";
        if (qty == 1) return "Last Item";
        if (qty <= 5) return "Limited Stock";
        return "In Stock";
    }

    private static string ResolveCategoryDisplay(Product source)
    {
        return source.Category switch
        {
            ProductCategory.Electronics => "Electronics & Technology",
            ProductCategory.Clothing => "Clothing & Fashion",
            ProductCategory.Books => "Books & Media",
            ProductCategory.Home => "Home & Garden",
            _ => "Uncategorized"
        };
    }

    private static string GetBrandFromDescriptionOrEmpty(string description)
    {
        return string.IsNullOrWhiteSpace(description) ? string.Empty : description;
    }
}