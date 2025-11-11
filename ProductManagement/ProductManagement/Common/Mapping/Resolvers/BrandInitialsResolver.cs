using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
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