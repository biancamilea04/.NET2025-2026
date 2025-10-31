using AutoMapper;
using ProductManagement.Features.Product;
using ProductManagement.Features.Product.DTOs;
using System.Globalization;

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
        return price.ToString("C", CultureInfo.CurrentCulture);
    }

    private static string ResolveProductAge(Product source)
    {
        var days = (DateTime.UtcNow - source.ReleaseDate).Days;
        return days switch
        {
            < 0 => "Not released",
            0 => "New",
            1 => "1 day",
            _ => $"{days} days"
        };
    }

    private static string ResolveBrandInitials(Product source)
    {
        var brandOrName = !string.IsNullOrWhiteSpace(source.Description) ? source.Description : source.Name;
        var parts = brandOrName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return string.Empty;
        return string.Concat(parts.Select(p => p[0])).ToUpperInvariant();
    }

    private static string ResolveAvailabilityStatus(Product source)
    {
        return source.IsAvaliable ? "Available" : "Unavailable";
    }

    private static string ResolveCategoryDisplay(Product source)
    {
        return source.Category switch
        {
            ProductCategory.Electronics => "Electronics",
            ProductCategory.Clothing => "Clothing",
            ProductCategory.Books => "Books",
            ProductCategory.Home => "Home",
            _ => "Other"
        };
    }

    private static string GetBrandFromDescriptionOrEmpty(string description)
    {
        return string.IsNullOrWhiteSpace(description) ? string.Empty : description;
    }
}