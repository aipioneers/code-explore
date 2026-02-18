using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Orders.Dtos;
using EnterpriseApp.Domain.Entities;
using EnterpriseApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Orders.Queries;

/// <summary>
/// Query to get a paginated list of orders.
/// </summary>
public record GetOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? CustomerId = null,
    string? Status = null,
    string? PaymentStatus = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? SortBy = null,
    bool SortDescending = true
) : IRequest<PagedList<OrderDto>>;

/// <summary>
/// Handler for GetOrdersQuery.
/// </summary>
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedList<OrderDto>>
{
    private readonly IRepository<Order> _orderRepository;

    public GetOrdersQueryHandler(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _orderRepository.AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(searchTerm) ||
                o.Customer.FirstName.ToLower().Contains(searchTerm) ||
                o.Customer.LastName.ToLower().Contains(searchTerm) ||
                (o.Customer.CompanyName != null && o.Customer.CompanyName.ToLower().Contains(searchTerm)));
        }

        // Apply customer filter
        if (request.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == request.CustomerId.Value);
        }

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        {
            query = query.Where(o => o.Status == status);
        }

        // Apply payment status filter
        if (!string.IsNullOrWhiteSpace(request.PaymentStatus) &&
            Enum.TryParse<Domain.Enums.PaymentStatus>(request.PaymentStatus, true, out var paymentStatus))
        {
            query = query.Where(o => o.PaymentStatus == paymentStatus);
        }

        // Apply date range filter
        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= request.ToDate.Value);
        }

        // Apply sorting
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "ordernumber" => request.SortDescending
                ? query.OrderByDescending(o => o.OrderNumber)
                : query.OrderBy(o => o.OrderNumber),
            "customer" => request.SortDescending
                ? query.OrderByDescending(o => o.Customer.LastName)
                : query.OrderBy(o => o.Customer.LastName),
            "status" => request.SortDescending
                ? query.OrderByDescending(o => o.Status)
                : query.OrderBy(o => o.Status),
            "total" => request.SortDescending
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),
            _ => request.SortDescending
                ? query.OrderByDescending(o => o.OrderDate)
                : query.OrderBy(o => o.OrderDate)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Get page
        var orders = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.CompanyName ?? $"{o.Customer.FirstName} {o.Customer.LastName}",
                Status = o.Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                OrderDate = o.OrderDate,
                Subtotal = o.Subtotal,
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Count,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedList<OrderDto>(orders, totalCount, request.PageNumber, request.PageSize);
    }
}
