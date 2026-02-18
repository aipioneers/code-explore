using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Represents an address for a customer.
/// </summary>
public class CustomerAddress : BaseEntity
{
    public CustomerAddress()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// The customer this address belongs to.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// The type of address.
    /// </summary>
    public AddressType Type { get; private set; }

    /// <summary>
    /// Street address line 1.
    /// </summary>
    public string Street { get; private set; } = string.Empty;

    /// <summary>
    /// Additional address line (e.g., apartment, suite).
    /// </summary>
    public string? AdditionalLine { get; private set; }

    /// <summary>
    /// City name.
    /// </summary>
    public string City { get; private set; } = string.Empty;

    /// <summary>
    /// Postal/ZIP code.
    /// </summary>
    public string PostalCode { get; private set; } = string.Empty;

    /// <summary>
    /// State or province.
    /// </summary>
    public string? State { get; private set; }

    /// <summary>
    /// Country code (ISO 3166-1 alpha-2).
    /// </summary>
    public string Country { get; private set; } = string.Empty;

    /// <summary>
    /// Indicates if this is the default address for its type.
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// Navigation property to the customer.
    /// </summary>
    public Customer Customer { get; private set; } = null!;

    /// <summary>
    /// Gets the formatted address.
    /// </summary>
    public string FormattedAddress => string.Join(", ",
        new[] { Street, AdditionalLine, PostalCode, City, State, Country }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    /// <summary>
    /// Creates a new customer address.
    /// </summary>
    public static CustomerAddress Create(
        Guid customerId,
        AddressType type,
        string street,
        string city,
        string postalCode,
        string country,
        string? additionalLine = null,
        bool isDefault = false)
    {
        return new CustomerAddress
        {
            CustomerId = customerId,
            Type = type,
            Street = street,
            City = city,
            PostalCode = postalCode,
            Country = country,
            AdditionalLine = additionalLine,
            IsDefault = isDefault
        };
    }

    /// <summary>
    /// Updates the address.
    /// </summary>
    public void Update(
        string street,
        string city,
        string postalCode,
        string country,
        string? additionalLine = null,
        string? state = null)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
        AdditionalLine = additionalLine;
        State = state;
    }

    /// <summary>
    /// Sets whether this is the default address.
    /// </summary>
    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }
}
