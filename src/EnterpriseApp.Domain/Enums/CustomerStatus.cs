namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Represents the status of a customer.
/// </summary>
public enum CustomerStatus
{
    /// <summary>
    /// Potential customer, not yet active.
    /// </summary>
    Prospect = 1,

    /// <summary>
    /// Active customer.
    /// </summary>
    Active = 2,

    /// <summary>
    /// Inactive customer.
    /// </summary>
    Inactive = 3,

    /// <summary>
    /// Customer account is suspended.
    /// </summary>
    Suspended = 4
}
