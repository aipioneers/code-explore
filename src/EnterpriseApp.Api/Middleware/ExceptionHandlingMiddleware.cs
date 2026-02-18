using System.Text.Json;
using EnterpriseApp.Domain.Exceptions;
using EnterpriseApp.Shared.Contracts;

namespace EnterpriseApp.Api.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and returning appropriate HTTP responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(
                    "Validation Error",
                    "One or more validation errors occurred.",
                    validationException.Errors)),

            EntityNotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                new ErrorResponse(
                    "Not Found",
                    notFoundException.Message)),

            UnauthorizedException => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse(
                    "Unauthorized",
                    "You are not authorized to access this resource.")),

            ForbiddenException => (
                StatusCodes.Status403Forbidden,
                new ErrorResponse(
                    "Forbidden",
                    "You do not have permission to perform this action.")),

            ConcurrencyException concurrencyException => (
                StatusCodes.Status409Conflict,
                new ErrorResponse(
                    "Conflict",
                    concurrencyException.Message)),

            BusinessRuleException businessException => (
                StatusCodes.Status422UnprocessableEntity,
                new ErrorResponse(
                    "Business Rule Violation",
                    businessException.Message,
                    new Dictionary<string, string[]> { { "RuleCode", new[] { businessException.RuleCode } } })),

            DomainException domainException => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(
                    "Domain Error",
                    domainException.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                CreateInternalServerError(exception))
        };

        // Log the exception
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Request resulted in {StatusCode}: {Message}", statusCode, exception.Message);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private ErrorResponse CreateInternalServerError(Exception exception)
    {
        if (_environment.IsDevelopment())
        {
            return new ErrorResponse(
                "Internal Server Error",
                exception.Message,
                new Dictionary<string, string[]>
                {
                    { "StackTrace", new[] { exception.StackTrace ?? string.Empty } }
                });
        }

        return new ErrorResponse(
            "Internal Server Error",
            "An unexpected error occurred. Please try again later.");
    }
}
