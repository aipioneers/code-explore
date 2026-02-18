namespace EnterpriseApp.Domain.Common;

/// <summary>
/// Interface for entities that support soft delete functionality.
/// Entities implementing this interface will not be physically deleted from the database,
/// but will be marked as deleted and filtered out from normal queries.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Indicates whether the entity has been soft deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// The UTC timestamp when the entity was soft deleted.
    /// </summary>
    DateTime? DeletedAt { get; }

    /// <summary>
    /// The identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; }

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    /// <param name="deletedBy">The identifier of the user performing the deletion.</param>
    void Delete(Guid deletedBy);

    /// <summary>
    /// Restores a soft-deleted entity.
    /// </summary>
    void Restore();
}
