namespace EnterpriseApp.Application.Common.Interfaces;

/// <summary>
/// Service for accessing current user information.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's identifier.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the current user's roles.
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user is in a specific role.
    /// </summary>
    /// <param name="role">The role to check.</param>
    /// <returns>True if the user is in the role, false otherwise.</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Checks if the current user has a specific permission.
    /// </summary>
    /// <param name="permission">The permission to check.</param>
    /// <returns>True if the user has the permission, false otherwise.</returns>
    bool HasPermission(string permission);
}
