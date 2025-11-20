using ProductManagement.Features;

namespace ProductManagement.Common.Logging;

/// <summary>
/// Contains event ID constants for structured logging throughout the Product Management system.
/// Used to categorize and filter logs by operation type.
/// </summary>
public static class LogEvents
{
    /// <summary>
    /// Event ID for product creation operation start.
    /// </summary>
    public const int ProductCreationStarted = 2001;
    
    /// <summary>
    /// Event ID for product validation failure.
    /// </summary>
    public const int ProductValidationFailed = 2002;
    
    /// <summary>
    /// Event ID for product creation completion with metrics.
    /// </summary>
    public const int ProductCreationCompleted = 2003;
    
    /// <summary>
    /// Event ID for database operation start.
    /// </summary>
    public const int DatabaseOperationStarted = 2004;
    
    /// <summary>
    /// Event ID for database operation completion.
    /// </summary>
    public const int DatabaseOperationCompleted = 2005;
    
    /// <summary>
    /// Event ID for cache operation performed.
    /// </summary>
    public const int CacheOperationPerformed = 2006;
    
    /// <summary>
    /// Event ID for SKU validation operation.
    /// </summary>
    public const int SKUValidationPerformed = 2007;
    
    /// <summary>
    /// Event ID for stock quantity validation operation.
    /// </summary>
    public const int StockValidationPerformed = 2008;
}

/// <summary>
/// Contains metrics collected during product creation operations.
/// Captures timing information and operational status for performance monitoring and debugging.
/// </summary>
/// <param name="OperationId">Unique identifier for the operation instance.</param>
/// <param name="ProductName">The name of the product being created.</param>
/// <param name="SKU">The SKU of the product being created.</param>
/// <param name="Category">The product category.</param>
/// <param name="ValidationDuration">Time taken to validate the product request.</param>
/// <param name="DatabaseSaveDuration">Time taken to persist the product to the database.</param>
/// <param name="TotalDuration">Total time for the entire operation.</param>
/// <param name="Success">Indicates whether the operation completed successfully.</param>
/// <param name="ErrorReason">Description of the error, if the operation failed. Null for successful operations.</param>
public record ProductCreationMetrics(
    string OperationId,
    string ProductName,
    string SKU,
    ProductCategory Category,
    TimeSpan ValidationDuration,
    TimeSpan DatabaseSaveDuration,
    TimeSpan TotalDuration,
    bool Success,
    string? ErrorReason
);
