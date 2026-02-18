namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Join entity between Role and Permission for many-to-many relationship.
/// </summary>
public class RolePermission
{
    /// <summary>
    /// The role's identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// The permission's identifier.
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// Navigation property to the role.
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Navigation property to the permission.
    /// </summary>
    public Permission Permission { get; set; } = null!;
}
