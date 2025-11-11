using AutoMapper;
using ProductManagement.Features;
using ProductManagement.Features.DTOs;

namespace ProductManagement.Common.Mapping.Resolvers;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
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