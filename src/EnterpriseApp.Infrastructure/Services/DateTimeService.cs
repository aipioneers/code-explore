using EnterpriseApp.Application.Common.Interfaces;

namespace EnterpriseApp.Infrastructure.Services;

/// <summary>
/// Implementation of IDateTimeService that provides current date and time.
/// </summary>
public class DateTimeService : IDateTimeService
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc />
    public DateTime Now => DateTime.Now;

    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    /// <inheritdoc />
    public TimeOnly TimeOfDay => TimeOnly.FromDateTime(DateTime.UtcNow);
}
