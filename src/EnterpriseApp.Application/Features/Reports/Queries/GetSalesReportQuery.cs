using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Reports.Dtos;
using EnterpriseApp.Domain.Entities;
using EnterpriseApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Reports.Queries;

/// <summary>
/// Query to get sales report.
/// </summary>
public record GetSalesReportQuery(
    DateTime FromDate,
    DateTime ToDate,
    bool IncludeDetails = true
) : IRequest<SalesReportDto>;

/// <summary>
/// Handler for GetSalesReportQuery.
/// </summary>
public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, SalesReportDto>
{
    private readonly IRepository<Order> _orderRepository;

    public GetSalesReportQueryHandler(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<SalesReportDto> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Where(o => o.OrderDate >= request.FromDate && o.OrderDate <= request.ToDate)
            .Where(o => o.Status != OrderStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var report = new SalesReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TotalSales = orders.Sum(o => o.TotalAmount),
            TotalOrders = orders.Count,
            AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
            TotalItemsSold = orders.SelectMany(o => o.Items).Sum(i => i.Quantity),
            TotalTax = orders.Sum(o => o.TaxAmount),
            TotalDiscount = orders.Sum(o => o.DiscountAmount)
        };

        if (request.IncludeDetails)
        {
            // Sales by day
            report.SalesByDay = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesByDayDto
                {
                    Date = g.Key,
                    Amount = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(s => s.Date)
                .ToList();

            // Top products
            report.TopProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => new { i.ProductId, i.ProductName, i.ProductSku })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductSku = g.Key.ProductSku,
                    QuantitySold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.LineTotalWithTax)
                })
                .OrderByDescending(p => p.Revenue)
                .Take(10)
                .ToList();

            // Top customers
            report.TopCustomers = orders
                .GroupBy(o => new { o.CustomerId, CustomerName = o.Customer.CompanyName ?? $"{o.Customer.FirstName} {o.Customer.LastName}" })
                .Select(g => new TopCustomerDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            // Sales by status
            report.SalesByStatus = orders
                .GroupBy(o => o.Status.ToString())
                .Select(g => new SalesByStatusDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(o => o.TotalAmount)
                })
                .ToList();
        }

        return report;
    }
}
