using AutoMapper;
using ProductManagement.Common.Mapping;
using ProductManagement.Features.DTOs;
using ProductManagement.Features.Request;
using ProductManagement.Persistence;
using ProductManagement.Common.Logging;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using FluentValidation;

namespace ProductManagement.Features.Products;

public class CreateProductHandler(IMapper mapper, ApplicationContext context , ILogger<CreateProductHandler> logger, IValidator<CreateProductProfileRequest> validator )
{
    public async Task<IResult> Handle(CreateProductProfileRequest request)
    {
        var operationId = Guid.NewGuid().ToString("N").Substring(0,8);
        var overall = Stopwatch.StartNew();

        using (logger.BeginScope(new Dictionary<string, object> { ["OperationId"] = operationId }))
        {
            logger.LogInformation(new EventId(LogEvents.ProductCreationStarted),
                "Starting product creation: {Name}, {Brand}, {Category}, {SKU}", request.Name, request.Brand, request.Category, request.SKU);

            try
            {
                var validation = Stopwatch.StartNew();
                
                var validationResult = validator.ValidateAsync(request);
                if (!validationResult.Result.IsValid)
                {
                    validation.Stop();
                    logger.LogWarning(new EventId(LogEvents.ProductValidationFailed), "Product validation failed for {Name} with errors: {Errors}", request.Name, validationResult.Result.Errors);
                    throw new ValidationException(validationResult.Result.Errors);
                }

                validation.Stop();

                var productEntity = mapper.Map<Product>(request);
                var product = mapper.Map<ProductProfileDto>(productEntity);

                logger.LogDebug(new EventId(LogEvents.DatabaseOperationStarted), "Starting database save for operation {OperationId}", operationId);
                var dbWatch = Stopwatch.StartNew();
                context.Products.Add(productEntity);
                context.SaveChanges();
                dbWatch.Stop();
                logger.LogInformation(new EventId(LogEvents.DatabaseOperationCompleted), "Completed database save for ProductId {ProductId} (Operation {OperationId})", productEntity.Id, operationId);

                logger.LogDebug(new EventId(LogEvents.CacheOperationPerformed), "Updated cache key {CacheKey} after creating product {ProductId}", "all_products", productEntity.Id);

                overall.Stop();

                var metrics = new ProductCreationMetrics(
                    operationId,
                    request.Name,
                    request.SKU,
                    request.Category,
                    validation.Elapsed,
                    dbWatch.Elapsed,
                    overall.Elapsed,
                    true,
                    null
                );

                logger.LogProductCreationMetrics(metrics);

                return Results.Ok(product);
            }
            catch (Exception ex)
            {
                overall.Stop();
                var failedMetrics = new ProductCreationMetrics(
                    operationId,
                    request.Name,
                    request.SKU,
                    request.Category,
                    TimeSpan.Zero,
                    TimeSpan.Zero,
                    overall.Elapsed,
                    false,
                    ex.Message
                );

                logger.LogProductCreationMetrics(failedMetrics);

                throw;
            }
        }
    }
}