namespace EnterpriseApp.Domain.Common;

/// <summary>
/// Interface for entities that track creation and modification audit information.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// The UTC timestamp when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// The identifier of the user who created the entity.
    /// </summary>
    Guid CreatedBy { get; }

    /// <summary>
    /// The UTC timestamp when the entity was last modified.
    /// </summary>
    DateTime? ModifiedAt { get; }

    /// <summary>
    /// The identifier of the user who last modified the entity.
    /// </summary>
    Guid? ModifiedBy { get; }

    /// <summary>
    /// Sets the creation audit information.
    /// </summary>
    /// <param name="createdBy">The identifier of the user creating the entity.</param>
    /// <param name="createdAt">The UTC timestamp of creation.</param>
    void SetCreated(Guid createdBy, DateTime createdAt);

    /// <summary>
    /// Sets the modification audit information.
    /// </summary>
    /// <param name="modifiedBy">The identifier of the user modifying the entity.</param>
    /// <param name="modifiedAt">The UTC timestamp of modification.</param>
    void SetModified(Guid modifiedBy, DateTime modifiedAt);
}
