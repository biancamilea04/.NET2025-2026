using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto productProfileDto, string destMember,
        ResolutionContext context)
    {
        if (!source.IsAvailable)
            return "Out of stock";

        return source.StockQuantity switch
        {
            0 => "Unavailable",
            1 => "Last Item",
            <= 5 => "Limited Stock",
            _ => "In Stock"
        };

    }
}