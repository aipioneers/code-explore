using System.Diagnostics;
using EnterpriseApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EnterpriseApp.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for logging request processing.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId?.ToString() ?? "Anonymous";
        var userName = _currentUserService.Name ?? "Unknown";

        _logger.LogInformation(
            "Handling {RequestName} for User {UserId} ({UserName})",
            requestName,
            userId,
            userName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            // Log warning for slow requests (> 500ms)
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "Long running request: {RequestName} ({ElapsedMilliseconds}ms) for User {UserId}",
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    userId);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms for User {UserId}: {ErrorMessage}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                userId,
                ex.Message);

            throw;
        }
    }
}
