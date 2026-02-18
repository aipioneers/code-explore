namespace EnterpriseApp.Domain.Common;

/// <summary>
/// Base class for all domain entities providing common functionality
/// including identity, audit tracking, soft delete, and optimistic concurrency.
/// </summary>
public abstract class BaseEntity : IAuditable, ISoftDelete
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Concurrency token for optimistic locking.
    /// </summary>
    public byte[] RowVersion { get; protected set; } = [];

    #region IAuditable Implementation

    /// <inheritdoc />
    public DateTime CreatedAt { get; protected set; }

    /// <inheritdoc />
    public Guid CreatedBy { get; protected set; }

    /// <inheritdoc />
    public DateTime? ModifiedAt { get; protected set; }

    /// <inheritdoc />
    public Guid? ModifiedBy { get; protected set; }

    /// <inheritdoc />
    public void SetCreated(Guid createdBy, DateTime createdAt)
    {
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    /// <inheritdoc />
    public void SetModified(Guid modifiedBy, DateTime modifiedAt)
    {
        ModifiedBy = modifiedBy;
        ModifiedAt = modifiedAt;
    }

    #endregion

    #region ISoftDelete Implementation

    /// <inheritdoc />
    public bool IsDeleted { get; protected set; }

    /// <inheritdoc />
    public DateTime? DeletedAt { get; protected set; }

    /// <inheritdoc />
    public Guid? DeletedBy { get; protected set; }

    /// <inheritdoc />
    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <inheritdoc />
    public void Restore()
    {
        if (!IsDeleted)
        {
            return;
        }

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    #endregion

    #region Domain Events

    /// <summary>
    /// Gets the domain events raised by this entity.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to be dispatched.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (Id == Guid.Empty || other.Id == Guid.Empty)
        {
            return false;
        }

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !(left == right);
    }

    #endregion
}

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The UTC timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
