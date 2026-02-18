using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Audit log entity for tracking all changes.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// User ID who made the change.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Username who made the change.
    /// </summary>
    public string? Username { get; private set; }

    /// <summary>
    /// Action type (Create, Update, Delete, etc.).
    /// </summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>
    /// Entity type that was changed.
    /// </summary>
    public string EntityType { get; private set; } = string.Empty;

    /// <summary>
    /// Entity ID that was changed.
    /// </summary>
    public string EntityId { get; private set; } = string.Empty;

    /// <summary>
    /// Old values (JSON).
    /// </summary>
    public string? OldValues { get; private set; }

    /// <summary>
    /// New values (JSON).
    /// </summary>
    public string? NewValues { get; private set; }

    /// <summary>
    /// Changed properties (JSON array).
    /// </summary>
    public string? ChangedProperties { get; private set; }

    /// <summary>
    /// IP address of the request.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// User agent of the request.
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// Additional context/metadata (JSON).
    /// </summary>
    public string? Metadata { get; private set; }

    private AuditLog() { }

    /// <summary>
    /// Creates a new audit log entry.
    /// </summary>
    public static AuditLog Create(
        string action,
        string entityType,
        string entityId,
        Guid? userId = null,
        string? username = null,
        string? oldValues = null,
        string? newValues = null,
        string? changedProperties = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadata = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Username = username,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedProperties = changedProperties,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Metadata = metadata,
            CreatedAt = DateTime.UtcNow
        };
    }
}
