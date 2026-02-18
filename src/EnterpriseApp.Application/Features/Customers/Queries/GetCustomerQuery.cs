using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Customers.Dtos;
using EnterpriseApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Customers.Queries;

/// <summary>
/// Query to get a customer by ID.
/// </summary>
public record GetCustomerQuery(Guid Id) : IRequest<Result<CustomerDetailDto>>;

/// <summary>
/// Handler for GetCustomerQuery.
/// </summary>
public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Result<CustomerDetailDto>>
{
    private readonly IRepository<Customer> _customerRepository;

    public GetCustomerQueryHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDetailDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository
            .AsNoTracking()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
        {
            return Result.Failure<CustomerDetailDto>("Customer not found.");
        }

        var dto = new CustomerDetailDto
        {
            Id = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CompanyName = customer.CompanyName,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Mobile = customer.Mobile,
            Fax = customer.Fax,
            Website = customer.Website,
            TaxId = customer.TaxId,
            Status = customer.Status.ToString(),
            Notes = customer.Notes,
            CreditLimit = customer.CreditLimit,
            PaymentTermsDays = customer.PaymentTermsDays,
            CreatedAt = customer.CreatedAt,
            ModifiedAt = customer.ModifiedAt,
            Addresses = customer.Addresses
                .Where(a => !a.IsDeleted)
                .Select(a => new CustomerAddressDto
                {
                    Id = a.Id,
                    Type = a.Type.ToString(),
                    Street = a.Street,
                    AdditionalLine = a.AdditionalLine,
                    City = a.City,
                    PostalCode = a.PostalCode,
                    State = a.State,
                    Country = a.Country,
                    IsDefault = a.IsDefault,
                    FormattedAddress = a.FormattedAddress
                })
                .ToList()
        };

        return Result.Success(dto);
    }
}
