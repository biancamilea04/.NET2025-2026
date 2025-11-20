using AutoMapper;
using ProductManagement.Common.Mapping.Resolvers;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;
using ProductManagement.Features.Request;
using System.Linq;

namespace ProductManagement.Common.Mapping;

/// <summary>
/// AutoMapper profile for advanced product mapping between entities and DTOs.
/// Defines all mapping configurations including value resolvers for computed properties.
/// </summary>
public class AdvancedProductMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the AdvancedProductMappingProfile class.
    /// Configures all product-related mapping rules including property transformations and value resolution.
    /// </summary>
    public AdvancedProductMappingProfile()
    {
        CreateMap<Product, CreateProductProfileRequest>();

        CreateMap<CreateProductProfileRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Category == ProductCategory.Home ? (src.Price * 0.9m) : src.Price ))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>  src.Category == ProductCategory.Home ? null : src.ImageUrl))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.StockQuantity > 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
   
        CreateMap<Product, ProductProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU))
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Category == ProductCategory.Home ? null : src.ImageUrl))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.StockQuantity > 0))
            .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom<PriceFormatterResolver>())
            .ForMember(dest => dest.ProductAge, opt => opt.MapFrom<ProductAgeResolver>())
            .ForMember(dest => dest.BrandInitials, opt => opt.MapFrom<BrandInitialsResolver>())
            .ForMember(dest => dest.AvailabilityStatus, opt => opt.MapFrom<AvailabilityStatusResolver>())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

    }
}