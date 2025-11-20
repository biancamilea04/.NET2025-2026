using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

/// <summary>
/// Resolves the availability status display string based on product stock levels.
/// Converts stock quantity into human-readable availability messages for customer display.
/// </summary>
public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Resolves the stock quantity to an availability status message.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="productProfileDto">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member name.</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>A status string indicating product availability.</returns>
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