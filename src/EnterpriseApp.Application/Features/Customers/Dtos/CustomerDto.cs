namespace EnterpriseApp.Application.Features.Customers.Dtos;

/// <summary>
/// Customer list item DTO.
/// </summary>
public class CustomerDto
{
    public Guid Id { get; set; }
    public string CustomerNumber { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Customer detail DTO with all information.
/// </summary>
public class CustomerDetailDto
{
    public Guid Id { get; set; }
    public string CustomerNumber { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Website { get; set; }
    public string? TaxId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal? CreditLimit { get; set; }
    public int PaymentTermsDays { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public IReadOnlyList<CustomerAddressDto> Addresses { get; set; } = Array.Empty<CustomerAddressDto>();
}

/// <summary>
/// Customer address DTO.
/// </summary>
public class CustomerAddressDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? AdditionalLine { get; set; }
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? State { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string FormattedAddress { get; set; } = string.Empty;
}

/// <summary>
/// Request to create a customer.
/// </summary>
public class CreateCustomerRequest
{
    public string? CompanyName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? TaxId { get; set; }
    public string? Notes { get; set; }
    public decimal? CreditLimit { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
}

/// <summary>
/// Request to update a customer.
/// </summary>
public class UpdateCustomerRequest
{
    public string? CompanyName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Website { get; set; }
    public string? TaxId { get; set; }
    public string? Notes { get; set; }
    public decimal? CreditLimit { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
}

/// <summary>
/// Request to add an address.
/// </summary>
public class AddAddressRequest
{
    public string Type { get; set; } = "Billing";
    public string Street { get; set; } = string.Empty;
    public string? AdditionalLine { get; set; }
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? State { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
