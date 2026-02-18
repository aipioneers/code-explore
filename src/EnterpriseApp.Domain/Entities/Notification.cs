using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// User notification entity.
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// User ID who receives the notification.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Notification title.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Notification message.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Notification type/category.
    /// </summary>
    public string Type { get; private set; } = string.Empty;

    /// <summary>
    /// Priority level (low, normal, high, urgent).
    /// </summary>
    public string Priority { get; private set; } = "normal";

    /// <summary>
    /// Whether the notification has been read.
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// When the notification was read.
    /// </summary>
    public DateTime? ReadAt { get; private set; }

    /// <summary>
    /// Link/URL associated with the notification.
    /// </summary>
    public string? Link { get; private set; }

    /// <summary>
    /// Related entity type.
    /// </summary>
    public string? EntityType { get; private set; }

    /// <summary>
    /// Related entity ID.
    /// </summary>
    public Guid? EntityId { get; private set; }

    /// <summary>
    /// Additional data (JSON).
    /// </summary>
    public string? Data { get; private set; }

    /// <summary>
    /// Expiration date.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    private Notification() { }

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    public static Notification Create(
        Guid userId,
        string title,
        string message,
        string type,
        string priority = "normal",
        string? link = null,
        string? entityType = null,
        Guid? entityId = null,
        string? data = null,
        DateTime? expiresAt = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Priority = priority,
            IsRead = false,
            Link = link,
            EntityType = entityType,
            EntityId = entityId,
            Data = data,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marks the notification as read.
    /// </summary>
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Marks the notification as unread.
    /// </summary>
    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
        ModifiedAt = DateTime.UtcNow;
    }
}
