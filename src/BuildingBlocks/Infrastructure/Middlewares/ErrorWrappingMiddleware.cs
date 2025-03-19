using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using OcelotApiGw.Exceptions;
using Serilog;

namespace Infrastructure.Middlewares;

public class ErrorWrappingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public ErrorWrappingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment env)
    {
        // var errorMsg = string.Empty;
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex, env);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
    {
        //More log stuff        
        HttpStatusCode code;
        string errorMessage;
        var stackTrace = String.Empty;

        var exceptionType = exception.GetType();

        // Handle the different exception types here
        if (exceptionType == typeof(BadRequestException))
        {
            code = HttpStatusCode.BadRequest;
            var ex = (BadRequestException)exception;
            errorMessage = ex.Message;
        }
        else if (exceptionType == typeof(NotFoundException))
        {
            code = HttpStatusCode.NotFound;
            var ex = (NotFoundException)exception;
            errorMessage = ex.Message;
        }
        else if (exceptionType == typeof(AuthenticationFailedException))
        {
            code = HttpStatusCode.BadRequest;
            var ex = (AuthenticationFailedException)exception;
            errorMessage = ex.Message;
        }
        else if (exceptionType == typeof(NotAuthorizedException))
        {
            code = HttpStatusCode.Unauthorized;
            var ex = (NotAuthorizedException)exception;
            errorMessage = ex.Message;
        }
        else if (exceptionType == typeof(OperationFailedException))
        {
            code = HttpStatusCode.BadRequest;
            var ex = (OperationFailedException)exception;
            errorMessage = ex.Message;
        }
        else if (exceptionType == typeof(ValidationException))
        {
            code = HttpStatusCode.BadRequest;
            var ex = (ValidationException)exception;
            errorMessage = ex.Message;
        }
        else
        {
            code = HttpStatusCode.InternalServerError;
            errorMessage = exception.Message;
            if (env.IsEnvironment("Development"))
                stackTrace = exception.StackTrace;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        var errorResponse = new ApiErrorResponse(errorMessage, context.Response.StatusCode, stackTrace);
        var serializedErrorResponse = JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(serializedErrorResponse);
    }
}

public class ApiErrorResponse
{
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
    public string? StackTrace { get; set; }

    public ApiErrorResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public ApiErrorResponse(string errorMessage, int statusCode)
    {
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public ApiErrorResponse(string errorMessage, int statusCode, string? stackTrace)
    {
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
        StackTrace = stackTrace;
    }
}