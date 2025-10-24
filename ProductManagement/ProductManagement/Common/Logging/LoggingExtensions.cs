using System;
using Microsoft.Extensions.Logging;
using ProductManagement.Features.Product;

namespace ProductManagement.Common.Logging;

public static class LoggingExtensions
{
    public static void LogProductCreationMetrics(this ILogger logger, ProductCreationMetrics metrics)
    {
        if (logger is null) throw new ArgumentNullException(nameof(logger));
        if (metrics is null) throw new ArgumentNullException(nameof(metrics));

        var eventIdValue = metrics.Success ? LogEvents.ProductCreationCompleted : LogEvents.ProductValidationFailed;
        var eventName = metrics.Success ? nameof(LogEvents.ProductCreationCompleted) : nameof(LogEvents.ProductValidationFailed);

        const string message = "EventId={EventId} ProductCreationMetrics: OperationId={OperationId} Name={ProductName} SKU={SKU} Category={Category} ValidationMs={ValidationMs} DatabaseSaveMs={DatabaseSaveMs} TotalMs={TotalMs} Success={Success} ErrorReason={ErrorReason}";

        var args = new object[]
        {
            eventIdValue,
            metrics.OperationId,
            metrics.ProductName,
            metrics.SKU,
            metrics.Category,
            metrics.ValidationDuration.TotalMilliseconds,
            metrics.DatabaseSaveDuration.TotalMilliseconds,
            metrics.TotalDuration.TotalMilliseconds,
            metrics.Success,
            metrics.ErrorReason ?? string.Empty
        };

        var ev = new EventId(eventIdValue, eventName);

        if (metrics.Success)
        {
            logger.LogInformation(ev, message, args);
        }
        else
        {
            logger.LogError(ev, message, args);
        }
    }
}
