using Microsoft.Extensions.Logging;
using System.Globalization;
using ProductManagement.Features;

namespace ProductManagement.Common.Logging;

/// <summary>
/// Extension methods for logging product-related operations.
/// Provides specialized logging methods for product creation metrics and structured logging.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logs comprehensive metrics for a product creation operation.
    /// Formats timing and status information into a structured log entry.
    /// </summary>
    /// <param name="logger">The logger instance to write the log message.</param>
    /// <param name="metrics">The ProductCreationMetrics object containing operation details.</param>
    public static void LogProductCreationMetrics(this ILogger logger, ProductCreationMetrics metrics)
    {
        var validationMs = metrics.ValidationDuration.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
        var dbMs = metrics.DatabaseSaveDuration.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
        var totalMs = metrics.TotalDuration.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

        var status = metrics.Success ? "Success" : "Failure";

        var message = "ProductCreationMetrics: OperationId={OperationId}, Name={Name}, SKU={SKU}, Category={Category}, ValidationMs={ValidationMs}, DatabaseMs={DatabaseMs}, TotalMs={TotalMs}, Status={Status}";

        if (!string.IsNullOrEmpty(metrics.ErrorReason))
        {
            message += ", Error={ErrorReason}";
            logger.Log(LogLevel.Error, new EventId(LogEvents.ProductCreationCompleted), message,
                metrics.OperationId, metrics.ProductName, metrics.SKU, metrics.Category, validationMs, dbMs, totalMs, status, metrics.ErrorReason);
            return;
        }

        logger.LogInformation(new EventId(LogEvents.ProductCreationCompleted), message,
            metrics.OperationId, metrics.ProductName, metrics.SKU, metrics.Category, validationMs, dbMs, totalMs, status);
    }
}