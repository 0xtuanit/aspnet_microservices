using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.SeedWork;

namespace Infrastructure.Middlewares;

public class ErrorWrappingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public ErrorWrappingMiddleware(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // var errorMsg = string.Empty;
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"An exception type: {ex.GetType()} has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        //More log stuff        
        HttpStatusCode code;

        // Handle the different exception types here
        switch (exception)
        {
            case BadHttpRequestException:
                code = HttpStatusCode.BadRequest;
                break;
            case ArgumentOutOfRangeException:
                code = HttpStatusCode.BadRequest;
                break;
            case ValidationException:
                code = HttpStatusCode.BadRequest;
                break;
            case TimeoutException:
                code = HttpStatusCode.GatewayTimeout;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        var errorResponse = new ApiErrorResult<object>(exception.Message);
        var serializedErrorResponse = JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(serializedErrorResponse);
    }
}