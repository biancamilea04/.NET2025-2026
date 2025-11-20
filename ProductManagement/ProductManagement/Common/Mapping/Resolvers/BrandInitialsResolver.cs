using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

/// <summary>
/// Resolves the brand initials from the brand name.
/// Extracts the first letter from each word in the brand name to create a concise abbreviation.
/// </summary>
public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Resolves the brand name to its initials representation.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="productProfileDto">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member name.</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>The initials of the brand name, or "?" if brand is null or empty.</returns>
    public string Resolve(Product source, ProductProfileDto productProfileDto, string destMember,
        ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.Brand))
            return "?";

        var words = source.Brand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var initials = string.Concat(words.Select(w => char.ToUpper(w[0])));

        return initials;
    }
}