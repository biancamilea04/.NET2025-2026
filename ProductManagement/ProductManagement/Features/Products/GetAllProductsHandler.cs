using ProductManagement.Features.Request;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class GetAllProductsHandler(ApplicationContext context , ILogger<CreateProductHandler> logger)
{
    public async Task<IResult> Handle()
    {
        var products = context.Products.ToList();
        logger.LogInformation("Retrieved {ProductCount} products from the database", products.Count);
        
        return Results.Ok(products);
    }
}