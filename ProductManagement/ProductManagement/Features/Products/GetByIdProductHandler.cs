using ProductManagement.Features.Request;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class GetByIdProductHandler(ApplicationContext context , ILogger<CreateProductHandler> logger)
{
    public async Task<IResult> Handler(GetByIdProductRequest request)
    {
        var product = await context.Products.FindAsync(request.Id);
        if (product == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found.", request.Id);
            return Results.NotFound();
        }
        
        logger.LogInformation("Product with ID {ProductId} found.", request.Id);
        return Results.Ok(product);
    }
}