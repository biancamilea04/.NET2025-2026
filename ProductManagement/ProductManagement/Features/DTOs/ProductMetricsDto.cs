namespace ProductManagement.Features.DTOs;

/// <summary>
/// Data Transfer Object for product metrics and inventory data.
/// Provides aggregated real-time product performance data including counts, inventory value, and statistics.
/// </summary>
public class ProductMetricsDto
{
    /// <summary>
    /// Gets or sets the total number of products in the system.
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Gets or sets the number of products created today.
    /// </summary>
    public int TodayCreatedCount { get; set; }

    /// <summary>
    /// Gets or sets the total inventory value calculated as sum of (Price * StockQuantity) for all products.
    /// </summary>
    public decimal TotalInventoryValue { get; set; }

    /// <summary>
    /// Gets or sets the breakdown of products by category.
    /// Key: Category name, Value: Count of products in that category.
    /// </summary>
    public Dictionary<string, int> ProductsByCategory { get; set; } = new();

    /// <summary>
    /// Gets or sets the breakdown of products by stock status.
    /// Includes: "Out of Stock", "Limited Stock", "In Stock".
    /// </summary>
    public Dictionary<string, int> StockStatusBreakdown { get; set; } = new();

    /// <summary>
    /// Gets or sets the average metrics across all products.
    /// Includes average price, stock quantity, and product age.
    /// </summary>
    public AverageMetrics AveragePerformance { get; set; } = new();

    /// <summary>
    /// Gets or sets the top 5 products by price (most expensive first).
    /// </summary>
    public List<TopProduct> TopProductsByPrice { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp when the metrics were generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents average metrics across all products in the system.
/// </summary>
public class AverageMetrics
{
    /// <summary>
    /// Gets or sets the average price of all products.
    /// </summary>
    public decimal AveragePrice { get; set; }

    /// <summary>
    /// Gets or sets the average stock quantity across all products.
    /// </summary>
    public double AverageStockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the average product age in days since release date.
    /// </summary>
    public double AverageProductAgeInDays { get; set; }
}

/// <summary>
/// Represents a top product by price in the metrics dashboard.
/// </summary>
public class TopProduct
{
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the brand or manufacturer name.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Stock Keeping Unit (SKU).
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the current stock quantity.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

