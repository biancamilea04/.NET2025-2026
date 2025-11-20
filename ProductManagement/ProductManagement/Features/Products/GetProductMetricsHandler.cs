using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManagement.Common.Logging;
using ProductManagement.Features.DTOs;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handler for retrieving product metrics and inventory aggregations.
/// Provides real-time dashboard data including product counts, inventory value, and performance statistics.
/// </summary>
public class GetProductMetricsHandler
{
    private readonly ApplicationContext _context;
    private readonly ILogger<GetProductMetricsHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetProductMetricsHandler class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="logger">The logger instance for tracking operations.</param>
    public GetProductMetricsHandler(
        ApplicationContext context,
        ILogger<GetProductMetricsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request to retrieve product metrics and returns aggregated data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A ProductMetricsDto containing all aggregated product metrics.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during metrics retrieval.</exception>
    public async Task<ProductMetricsDto> Handle(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                new EventId(LogEvents.ProductCreationStarted, "ProductMetricsStarted"),
                "Fetching product metrics dashboard data");

            // Retrieve all products from database
            var products = await _context.Products.ToListAsync(cancellationToken);

            if (products.Count == 0)
            {
                _logger.LogWarning(
                    new EventId(LogEvents.ProductValidationFailed, "NoProductsFound"),
                    "No products found in the system for metrics calculation");
            }

            var today = DateTime.UtcNow.Date;

            // Build metrics DTO with aggregated data
            var metrics = new ProductMetricsDto
            {
                TotalProducts = products.Count,
                TodayCreatedCount = products.Count(p => p.CreatedAt.Date == today),
                TotalInventoryValue = products.Sum(p => p.Price * p.StockQuantity),

                // Group products by category
                ProductsByCategory = products
                    .GroupBy(p => p.Category.ToString())
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count()),

                // Calculate stock status breakdown
                StockStatusBreakdown = new Dictionary<string, int>
                {
                    ["Out of Stock"] = products.Count(p => p.StockQuantity == 0),
                    ["Limited Stock"] = products.Count(p => p.StockQuantity > 0 && p.StockQuantity <= 10),
                    ["In Stock"] = products.Count(p => p.StockQuantity > 10)
                },

                // Calculate average metrics
                AveragePerformance = new AverageMetrics
                {
                    AveragePrice = products.Any() ? products.Average(p => p.Price) : 0,
                    AverageStockQuantity = products.Any() ? products.Average(p => p.StockQuantity) : 0,
                    AverageProductAgeInDays = products.Any()
                        ? products.Average(p => (DateTime.UtcNow - p.ReleaseDate).TotalDays)
                        : 0
                },

                // Get top 5 products by price
                TopProductsByPrice = products
                    .OrderByDescending(p => p.Price)
                    .Take(5)
                    .Select(p => new TopProduct
                    {
                        Name = p.Name,
                        Brand = p.Brand,
                        SKU = p.SKU,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        Category = p.Category.ToString()
                    })
                    .ToList()
            };

            _logger.LogInformation(
                new EventId(LogEvents.ProductCreationCompleted, "ProductMetricsCompleted"),
                "Product metrics dashboard data fetched successfully. " +
                "Total products: {TotalCount}, Today created: {TodayCount}, " +
                "Total inventory value: ${InventoryValue:F2}, Average price: ${AvgPrice:F2}",
                metrics.TotalProducts,
                metrics.TodayCreatedCount,
                metrics.TotalInventoryValue,
                metrics.AveragePerformance.AveragePrice);

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                new EventId(LogEvents.DatabaseOperationCompleted, "ProductMetricsError"),
                ex,
                "Error fetching product metrics dashboard");
            throw;
        }
    }
}
