namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Join entity between User and Role for many-to-many relationship.
/// </summary>
public class UserRole
{
    /// <summary>
    /// The user's identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The role's identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// The date when the role was assigned.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The user who assigned this role (optional).
    /// </summary>
    public Guid? AssignedBy { get; set; }

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Navigation property to the role.
    /// </summary>
    public Role Role { get; set; } = null!;
}
