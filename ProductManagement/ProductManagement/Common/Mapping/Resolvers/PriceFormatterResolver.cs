using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

/// <summary>
/// Resolves the formatted price display string with currency formatting.
/// Converts a decimal price to a locale-appropriate currency string.
/// </summary>
public class PriceFormatterResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Resolves the product price to a formatted currency string.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="productProfileDto">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member name.</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>The price formatted as a currency string.</returns>
    public string Resolve(Product source, ProductProfileDto productProfileDto, string destMember,
        ResolutionContext context)
        => source.Price.ToString("C2");
}