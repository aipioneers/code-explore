namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Order status enumeration.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is pending confirmation.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Order has been confirmed.
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Order is being processed.
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Order has been shipped.
    /// </summary>
    Shipped = 3,

    /// <summary>
    /// Order has been delivered.
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// Order has been cancelled.
    /// </summary>
    Cancelled = 5
}

/// <summary>
/// Payment status enumeration.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment is pending.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Payment has been received.
    /// </summary>
    Paid = 1,

    /// <summary>
    /// Payment failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Payment has been refunded.
    /// </summary>
    Refunded = 3,

    /// <summary>
    /// Partial payment received.
    /// </summary>
    PartiallyPaid = 4
}
