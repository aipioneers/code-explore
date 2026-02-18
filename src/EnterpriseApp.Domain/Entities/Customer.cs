using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Represents a customer in the system.
/// </summary>
public class Customer : BaseEntity
{
    private readonly List<CustomerAddress> _addresses = [];

    public Customer()
    {
        Id = Guid.NewGuid();
        Status = CustomerStatus.Active;
    }

    /// <summary>
    /// Unique customer number (e.g., "CUST-00001").
    /// </summary>
    public string CustomerNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Company name (for business customers).
    /// </summary>
    public string? CompanyName { get; private set; }

    /// <summary>
    /// Contact person's first name.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// Contact person's last name.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Primary email address.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Primary phone number.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Mobile phone number.
    /// </summary>
    public string? Mobile { get; private set; }

    /// <summary>
    /// Fax number.
    /// </summary>
    public string? Fax { get; private set; }

    /// <summary>
    /// Website URL.
    /// </summary>
    public string? Website { get; private set; }

    /// <summary>
    /// Tax identification number (Steuernummer/USt-IdNr).
    /// </summary>
    public string? TaxId { get; private set; }

    /// <summary>
    /// Customer status.
    /// </summary>
    public CustomerStatus Status { get; private set; }

    /// <summary>
    /// Internal notes about the customer.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Credit limit for the customer.
    /// </summary>
    public decimal? CreditLimit { get; private set; }

    /// <summary>
    /// Payment terms in days.
    /// </summary>
    public int PaymentTermsDays { get; private set; } = 30;

    /// <summary>
    /// Customer's addresses.
    /// </summary>
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    /// <summary>
    /// Gets the customer's display name.
    /// </summary>
    public string DisplayName => !string.IsNullOrEmpty(CompanyName)
        ? CompanyName
        : $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Gets the customer's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    public static Customer Create(
        string customerNumber,
        string firstName,
        string lastName,
        string email,
        string? companyName = null,
        string? phone = null)
    {
        return new Customer
        {
            CustomerNumber = customerNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLowerInvariant(),
            CompanyName = companyName,
            Phone = phone
        };
    }

    /// <summary>
    /// Updates the customer's information.
    /// </summary>
    public void Update(
        string firstName,
        string lastName,
        string email,
        string? companyName,
        string? phone,
        string? mobile,
        string? fax,
        string? website,
        string? taxId,
        string? notes,
        decimal? creditLimit,
        int paymentTermsDays)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant();
        CompanyName = companyName;
        Phone = phone;
        Mobile = mobile;
        Fax = fax;
        Website = website;
        TaxId = taxId;
        Notes = notes;
        CreditLimit = creditLimit;
        PaymentTermsDays = paymentTermsDays;
    }

    /// <summary>
    /// Activates the customer.
    /// </summary>
    public void Activate()
    {
        Status = CustomerStatus.Active;
    }

    /// <summary>
    /// Deactivates the customer.
    /// </summary>
    public void Deactivate()
    {
        Status = CustomerStatus.Inactive;
    }

    /// <summary>
    /// Marks the customer as a prospect.
    /// </summary>
    public void MarkAsProspect()
    {
        Status = CustomerStatus.Prospect;
    }

    /// <summary>
    /// Adds an address to the customer.
    /// </summary>
    public CustomerAddress AddAddress(
        AddressType type,
        string street,
        string city,
        string postalCode,
        string country,
        string? additionalLine = null,
        bool isDefault = false)
    {
        // If this is the first address or isDefault is true, set as default
        if (isDefault || !_addresses.Any())
        {
            foreach (var addr in _addresses.Where(a => a.Type == type))
            {
                addr.SetDefault(false);
            }
        }

        var address = CustomerAddress.Create(
            Id,
            type,
            street,
            city,
            postalCode,
            country,
            additionalLine,
            isDefault || !_addresses.Any(a => a.Type == type));

        _addresses.Add(address);
        return address;
    }

    /// <summary>
    /// Removes an address from the customer.
    /// </summary>
    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address != null)
        {
            _addresses.Remove(address);
        }
    }

    /// <summary>
    /// Gets the default billing address.
    /// </summary>
    public CustomerAddress? GetDefaultBillingAddress()
    {
        return _addresses.FirstOrDefault(a => a.Type == AddressType.Billing && a.IsDefault)
            ?? _addresses.FirstOrDefault(a => a.Type == AddressType.Billing);
    }

    /// <summary>
    /// Gets the default shipping address.
    /// </summary>
    public CustomerAddress? GetDefaultShippingAddress()
    {
        return _addresses.FirstOrDefault(a => a.Type == AddressType.Shipping && a.IsDefault)
            ?? _addresses.FirstOrDefault(a => a.Type == AddressType.Shipping)
            ?? GetDefaultBillingAddress();
    }
}
