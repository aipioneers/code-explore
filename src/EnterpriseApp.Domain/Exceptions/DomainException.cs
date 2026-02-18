namespace EnterpriseApp.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-level errors.
/// </summary>
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityType { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityType, object entityId)
        : base($"{entityType} with ID '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public class BusinessRuleException : DomainException
{
    public string RuleCode { get; }

    public BusinessRuleException(string ruleCode, string message)
        : base(message)
    {
        RuleCode = ruleCode;
    }
}

/// <summary>
/// Exception thrown when there is a concurrency conflict.
/// </summary>
public class ConcurrencyException : DomainException
{
    public string EntityType { get; }
    public object EntityId { get; }

    public ConcurrencyException(string entityType, object entityId)
        : base($"A concurrency conflict occurred while updating {entityType} with ID '{entityId}'. " +
               "The entity has been modified by another user. Please refresh and try again.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}

/// <summary>
/// Exception thrown when a validation rule fails.
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : this()
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : this(new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        })
    {
    }
}

/// <summary>
/// Exception thrown when an unauthorized operation is attempted.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException()
        : base("You are not authorized to perform this action.")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when a forbidden operation is attempted.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException()
        : base("You do not have permission to access this resource.")
    {
    }

    public ForbiddenException(string message)
        : base(message)
    {
    }
}
