using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

/// <summary>
/// Resolves the category display name with a user-friendly formatted string.
/// Converts ProductCategory enum values to descriptive display names for UI presentation.
/// </summary>
public class CategoryDisplayResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Resolves the product category to a display-friendly name.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="productProfileDto">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member name.</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>A formatted category display name.</returns>
    public string Resolve(Product source, ProductProfileDto productProfileDto, string destMember,
        ResolutionContext context)
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
}