namespace ProductManagement.Features.Request;

/// <summary>
/// Request model for retrieving a product by its unique identifier.
/// Used to fetch a single product from the Product Management system.
/// </summary>
/// <param name="Id">The unique identifier (GUID) of the product to retrieve.</param>
public record GetByIdProductRequest(Guid Id);