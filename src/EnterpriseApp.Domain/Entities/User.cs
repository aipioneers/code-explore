using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User : BaseEntity
{
    private readonly List<UserRole> _userRoles = [];

    public User()
    {
        Id = Guid.NewGuid();
        Status = UserStatus.Active;
        TwoFactorEnabled = false;
        FailedLoginAttempts = 0;
    }

    /// <summary>
    /// The user's email address (unique identifier for login).
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// The hashed password.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// The user's first name.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// The user's last name.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// The user's phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// The user's current status.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Indicates whether two-factor authentication is enabled.
    /// </summary>
    public bool TwoFactorEnabled { get; private set; }

    /// <summary>
    /// The TOTP secret key for 2FA (encrypted).
    /// </summary>
    public string? TwoFactorSecret { get; private set; }

    /// <summary>
    /// The number of consecutive failed login attempts.
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// The lockout end date (if account is locked).
    /// </summary>
    public DateTime? LockoutEnd { get; private set; }

    /// <summary>
    /// The last login timestamp.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// The current refresh token.
    /// </summary>
    public string? RefreshToken { get; private set; }

    /// <summary>
    /// The refresh token expiry date.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    /// <summary>
    /// Password reset token.
    /// </summary>
    public string? PasswordResetToken { get; private set; }

    /// <summary>
    /// Password reset token expiry.
    /// </summary>
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    /// <summary>
    /// The user's assigned roles.
    /// </summary>
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public static User Create(string email, string passwordHash, string firstName, string lastName, string? phoneNumber = null)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber
        };

        return user;
    }

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }

    /// <summary>
    /// Records a successful login.
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    /// <summary>
    /// Records a failed login attempt.
    /// </summary>
    public void RecordFailedLogin(int maxAttempts = 5, int lockoutMinutes = 15)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= maxAttempts)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
        }
    }

    /// <summary>
    /// Checks if the account is locked.
    /// </summary>
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    /// <summary>
    /// Enables two-factor authentication.
    /// </summary>
    public void EnableTwoFactor(string secret)
    {
        TwoFactorSecret = secret;
        TwoFactorEnabled = true;
    }

    /// <summary>
    /// Disables two-factor authentication.
    /// </summary>
    public void DisableTwoFactor()
    {
        TwoFactorSecret = null;
        TwoFactorEnabled = false;
    }

    /// <summary>
    /// Sets the refresh token.
    /// </summary>
    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    /// <summary>
    /// Clears the refresh token (logout).
    /// </summary>
    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    /// <summary>
    /// Sets the password reset token.
    /// </summary>
    public void SetPasswordResetToken(string token, int expiryHours = 24)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(expiryHours);
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        Status = UserStatus.Active;
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        ClearRefreshToken();
    }

    /// <summary>
    /// Adds a role to the user.
    /// </summary>
    public void AddRole(Role role)
    {
        if (_userRoles.All(ur => ur.RoleId != role.Id))
        {
            _userRoles.Add(new UserRole { UserId = Id, RoleId = role.Id, Role = role });
        }
    }

    /// <summary>
    /// Removes a role from the user.
    /// </summary>
    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
        }
    }
}
