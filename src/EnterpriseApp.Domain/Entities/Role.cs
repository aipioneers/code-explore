using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Represents a role that can be assigned to users.
/// </summary>
public class Role : BaseEntity
{
    private readonly List<UserRole> _userRoles = [];
    private readonly List<RolePermission> _rolePermissions = [];

    public Role()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// The unique name of the role.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// A description of the role.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Indicates whether this is a system role that cannot be deleted.
    /// </summary>
    public bool IsSystemRole { get; private set; }

    /// <summary>
    /// Indicates whether 2FA is required for users with this role.
    /// </summary>
    public bool RequiresTwoFactor { get; private set; }

    /// <summary>
    /// Users assigned to this role.
    /// </summary>
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    /// <summary>
    /// Permissions assigned to this role.
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    /// <summary>
    /// Creates a new role.
    /// </summary>
    public static Role Create(string name, string? description = null, bool isSystemRole = false, bool requiresTwoFactor = false)
    {
        return new Role
        {
            Name = name,
            Description = description,
            IsSystemRole = isSystemRole,
            RequiresTwoFactor = requiresTwoFactor
        };
    }

    /// <summary>
    /// Updates the role's information.
    /// </summary>
    public void Update(string name, string? description, bool requiresTwoFactor)
    {
        Name = name;
        Description = description;
        RequiresTwoFactor = requiresTwoFactor;
    }

    /// <summary>
    /// Adds a permission to the role.
    /// </summary>
    public void AddPermission(Permission permission)
    {
        if (_rolePermissions.All(rp => rp.PermissionId != permission.Id))
        {
            _rolePermissions.Add(new RolePermission { RoleId = Id, PermissionId = permission.Id, Permission = permission });
        }
    }

    /// <summary>
    /// Removes a permission from the role.
    /// </summary>
    public void RemovePermission(Guid permissionId)
    {
        var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            _rolePermissions.Remove(rolePermission);
        }
    }
}
