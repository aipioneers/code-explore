using EnterpriseApp.Application.Common;

namespace EnterpriseApp.Application.Common.Interfaces;

/// <summary>
/// Service for identity and authentication operations.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    Task<Result<AuthenticationResult>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a two-factor authentication code.
    /// </summary>
    Task<Result<AuthenticationResult>> VerifyTwoFactorAsync(Guid userId, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user by invalidating their refresh token.
    /// </summary>
    Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a password reset for a user.
    /// </summary>
    Task<Result<string>> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables two-factor authentication for a user.
    /// </summary>
    Task<Result<TwoFactorSetupResult>> EnableTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables two-factor authentication for a user.
    /// </summary>
    Task<Result> DisableTwoFactorAsync(Guid userId, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    Task<Result<UserInfo>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is in a specific role.
    /// </summary>
    Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an authentication operation.
/// </summary>
public class AuthenticationResult
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public bool RequiresTwoFactor { get; set; }
}

/// <summary>
/// Result of setting up two-factor authentication.
/// </summary>
public class TwoFactorSetupResult
{
    public string Secret { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
    public IReadOnlyList<string> RecoveryCodes { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Basic user information.
/// </summary>
public class UserInfo
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
