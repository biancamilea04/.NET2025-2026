using ProductManagement.Features.Request;
using ProductManagement.Persistence;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handler for retrieving all products from the database.
/// Manages the business logic for fetching the complete product collection.
/// </summary>
public class GetAllProductsHandler(ApplicationContext context, ILogger<GetAllProductsHandler> logger)
{
    /// <summary>
    /// Retrieves all products from the database asynchronously.
    /// </summary>
    /// <returns>An <see cref="IResult"/> containing either all products or an error response.</returns>
    public async Task<IResult> Handle()
    {
        try
        {
            logger.LogInformation("Starting retrieval of all products from database");
            
            var products = context.Products.ToList();
            
            logger.LogInformation("Successfully retrieved {ProductCount} products from the database", products.Count);
            return Results.Ok(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving all products from the database");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}