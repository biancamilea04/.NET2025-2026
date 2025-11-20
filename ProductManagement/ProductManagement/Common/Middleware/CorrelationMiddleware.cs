using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Common.Middelware;

/// <summary>
/// Middleware for managing HTTP request correlation IDs.
/// Ensures each request has a unique correlation ID for distributed tracing and log correlation.
/// </summary>
public class CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger)
{
    private const string HeaderName = "X-Correlation-ID";

    /// <summary>
    /// Processes an HTTP request and ensures a correlation ID is set.
    /// Creates a new correlation ID if none is provided, and includes it in the response headers.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N").Substring(0, 8);
            logger.LogDebug("Generated new correlation id: {CorrelationId}", (object)correlationId);
        }
        
        context.Response.OnStarting(() => {
            if (!context.Response.Headers.ContainsKey(HeaderName))
            {
                context.Response.Headers.Append(HeaderName, correlationId.ToString());
            }
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId.ToString() }))
        {
            await next(context);
        }
    }
}