using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Common.Middelware;

public class CollerationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CollerationMiddleware> _logger;
    private const string HeaderName = "X-Correlation-ID";

    public CollerationMiddleware(RequestDelegate next, ILogger<CollerationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation id from incoming header, otherwise generate one
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N").Substring(0, 8);
            _logger.LogDebug("Generated new correlation id: {CorrelationId}", correlationId);
        }

        // Ensure the response header contains the correlation id
        context.Response.OnStarting(() => {
            if (!context.Response.Headers.ContainsKey(HeaderName))
            {
                context.Response.Headers.Add(HeaderName, correlationId.ToString());
            }
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId.ToString() }))
        {
            await _next(context);
        }
    }
}