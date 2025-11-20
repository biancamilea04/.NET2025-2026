using Microsoft.Extensions.Logging;
using System.Globalization;
using ProductManagement.Features;

namespace ProductManagement.Common.Logging;

public static class LoggingExtensions
{
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