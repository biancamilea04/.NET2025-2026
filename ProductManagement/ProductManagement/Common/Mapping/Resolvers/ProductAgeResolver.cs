using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

/// <summary>
/// Resolves the product age display string based on the release date.
/// Converts elapsed time since release into human-readable categories (New Release, months old, years old, Classic, Vintage).
/// </summary>
public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Resolves the product age from the release date to a display string.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="productProfileDto">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member name.</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>A human-readable string describing the product's age.</returns>
    public string Resolve(Product source, ProductProfileDto productProfileDto, string destMember,
        ResolutionContext context)
    {
        var ageInDays = (DateTime.UtcNow - source.ReleaseDate).TotalDays;
        if (ageInDays < 30)
            return "New Release";
        if (ageInDays < 365)
        {
            var months = (int)(ageInDays / 30);
            return $"{months} month{(months > 1 ? "s" : "")} old";
        }

        if (ageInDays < 1825)
        {
            var years = (int)(ageInDays / 365);
            return $"{years} year{(years > 1 ? "s" : "")} old";
        }

        if (ageInDays == 1825)
        {
            return "Classic";
        }
        
        return "Vintage";
    }
}