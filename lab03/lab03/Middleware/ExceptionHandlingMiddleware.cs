using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace lab03.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int status = (int)HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";

        if (exception is ApiException apiEx)
        {
            status = apiEx.StatusCode;
            message = apiEx.Message;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;

        var payload = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(payload);
    }
}