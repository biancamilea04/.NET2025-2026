using AutoMapper;
using ProductManagement.Common.Mapping;
using ProductManagement.Features.DTOs;
using ProductManagement.Features.Request;
using ProductManagement.Persistence;

namespace ProductManagement.Features;

public class CreateProductHandler(IMapper mapper, ApplicationContext context , ILogger<CreateProductHandler> logger )
{
    public ProductProfileDto Handle(CreateProductProfileRequest request)
    {
        logger.LogInformation("Creating a new product profile: {ProductName}, {ProductBrand}, {ProductCategory}, {ProductSKU}", request.Name, request.Brand, request.Category, request.SKU);

        var product = mapper.Map<ProductProfileDto>(request);
        var productEntity = mapper.Map<Product>(request);
        context.Products.Add(productEntity);
        context.SaveChanges();
        
        //TODO: ● [ ] Update cache key to "all_products"
        //      ● [ ] Maintain existing functionality (validation, caching, exception handling)

        return product;
    }
}