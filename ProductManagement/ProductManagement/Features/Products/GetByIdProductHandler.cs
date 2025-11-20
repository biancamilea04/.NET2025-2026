using ProductManagement.Features.Request;
using ProductManagement.Persistence;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handler for retrieving a product by its unique identifier.
/// Manages the business logic for fetching a single product from the database.
/// </summary>
public class GetByIdProductHandler(ApplicationContext context, ILogger<GetByIdProductHandler> logger)
{
    /// <summary>
    /// Retrieves a single product by its ID asynchronously.
    /// </summary>
    /// <param name="request">The request containing the product ID to retrieve.</param>
    /// <returns>An <see cref="IResult"/> containing the product if found, or a 404 Not Found response.</returns>
    public async Task<IResult> Handle(GetByIdProductRequest request)
    {
        try
        {
            if (request == null || request.Id == Guid.Empty)
            {
                logger.LogWarning("GetById request received with invalid ID: {ProductId}", request?.Id);
                return Results.BadRequest("Product ID cannot be empty.");
            }

            logger.LogInformation("Attempting to retrieve product with ID {ProductId}", request.Id);
            
            var product = await context.Products.FindAsync(request.Id);
            
            if (product == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found", request.Id);
                return Results.NotFound($"Product with ID {request.Id} was not found.");
            }
            
            logger.LogInformation("Successfully retrieved product with ID {ProductId}", request.Id);
            return Results.Ok(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving product with ID {ProductId}", request?.Id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}