using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Represents a permission that can be assigned to roles.
/// </summary>
public class Permission : BaseEntity
{
    private readonly List<RolePermission> _rolePermissions = [];

    public Permission()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// The unique code of the permission (e.g., "customers:read").
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// A human-readable name for the permission.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// A description of what this permission allows.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The category/module this permission belongs to.
    /// </summary>
    public string Category { get; private set; } = string.Empty;

    /// <summary>
    /// Roles that have this permission.
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    public static Permission Create(string code, string name, string category, string? description = null)
    {
        return new Permission
        {
            Code = code,
            Name = name,
            Category = category,
            Description = description
        };
    }
}
