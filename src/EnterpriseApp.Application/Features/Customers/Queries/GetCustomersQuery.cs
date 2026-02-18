using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Customers.Dtos;
using EnterpriseApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Customers.Queries;

/// <summary>
/// Query to get a paginated list of customers.
/// </summary>
public record GetCustomersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Status = null,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PagedList<CustomerDto>>;

/// <summary>
/// Handler for GetCustomersQuery.
/// </summary>
public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedList<CustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;

    public GetCustomersQueryHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = _customerRepository.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.CustomerNumber.ToLower().Contains(searchTerm) ||
                c.FirstName.ToLower().Contains(searchTerm) ||
                c.LastName.ToLower().Contains(searchTerm) ||
                (c.CompanyName != null && c.CompanyName.ToLower().Contains(searchTerm)) ||
                c.Email.ToLower().Contains(searchTerm));
        }

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<Domain.Enums.CustomerStatus>(request.Status, true, out var status))
        {
            query = query.Where(c => c.Status == status);
        }

        // Apply sorting
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "customernumber" => request.SortDescending
                ? query.OrderByDescending(c => c.CustomerNumber)
                : query.OrderBy(c => c.CustomerNumber),
            "companyname" => request.SortDescending
                ? query.OrderByDescending(c => c.CompanyName)
                : query.OrderBy(c => c.CompanyName),
            "email" => request.SortDescending
                ? query.OrderByDescending(c => c.Email)
                : query.OrderBy(c => c.Email),
            "createdat" => request.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            _ => request.SortDescending
                ? query.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName)
                : query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Get page
        var customers = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                CustomerNumber = c.CustomerNumber,
                DisplayName = c.CompanyName ?? $"{c.FirstName} {c.LastName}",
                CompanyName = c.CompanyName,
                FullName = $"{c.FirstName} {c.LastName}",
                Email = c.Email,
                Phone = c.Phone,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedList<CustomerDto>(customers, totalCount, request.PageNumber, request.PageSize);
    }
}
