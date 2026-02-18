namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Represents the status of a user account.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// The account is active and can be used.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The account is inactive and cannot login.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// The account is pending email verification.
    /// </summary>
    PendingVerification = 3,

    /// <summary>
    /// The account is locked due to too many failed login attempts.
    /// </summary>
    Locked = 4
}
