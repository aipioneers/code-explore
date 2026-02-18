namespace EnterpriseApp.Shared.Contracts;

/// <summary>
/// Standard error response format.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The error type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed validation errors.
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// A unique trace ID for this request.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Creates a new error response.
    /// </summary>
    public ErrorResponse()
    {
    }

    /// <summary>
    /// Creates a new error response with type and message.
    /// </summary>
    public ErrorResponse(string type, string message)
    {
        Type = type;
        Message = message;
    }

    /// <summary>
    /// Creates a new error response with type, message, and errors.
    /// </summary>
    public ErrorResponse(string type, string message, IDictionary<string, string[]>? errors)
    {
        Type = type;
        Message = message;
        Errors = errors;
    }
}
