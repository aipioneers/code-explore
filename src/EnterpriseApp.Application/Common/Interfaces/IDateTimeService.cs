namespace EnterpriseApp.Application.Common.Interfaces;

/// <summary>
/// Service for accessing date and time, allowing for easier testing.
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets today's date in UTC.
    /// </summary>
    DateOnly Today { get; }

    /// <summary>
    /// Gets the current time of day in UTC.
    /// </summary>
    TimeOnly TimeOfDay { get; }
}
