using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Common.Middelware;

public class CorrerationMiddleware(RequestDelegate next, ILogger<CorrerationMiddleware> logger)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N").Substring(0, 8);
            logger.LogDebug("Generated new correlation id: {CorrelationId}", correlationId);
        }
        
        context.Response.OnStarting(() => {
            if (!context.Response.Headers.ContainsKey(HeaderName))
            {
                context.Response.Headers.Add(HeaderName, correlationId.ToString());
            }
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId.ToString() }))
        {
            await next(context);
        }
    }
}