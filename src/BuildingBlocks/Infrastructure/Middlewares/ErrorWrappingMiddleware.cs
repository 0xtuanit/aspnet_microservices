using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Infrastructure.Middlewares.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken = new())
    {
        _logger.Error("Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);

        //More log stuff        

        // Handle the different exception types here
        (string Detail, string Title, int StatusCode) details = exception switch
        {
            InternalServerException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),
            ValidationException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            BadRequestException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            NotFoundException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status404NotFound
            ),
            _ =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };

        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Detail = details.Detail,
            Status = details.StatusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("ValidationErrors", validationException.ValidationResult.ErrorMessage);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status ?? 500;

        // var errors = (string)problemDetails.Extensions["ValidationErrors"]!;

        var errorResponse = new ApiErrorResult<string>(problemDetails.Detail);
        var errorList = new List<string>();
        errorList.Add(problemDetails.Title);

        // foreach (var (k, v) in problemDetails.Extensions)
        // {
        //     errorList.Add(err.Value.ToString());
        // }

        errorResponse.Errors = errorList;

        var serializedErrorResponse = JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(serializedErrorResponse, cancellationToken: cancellationToken);

        // await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
    }
}