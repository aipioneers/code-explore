using EnterpriseApp.Application.Features.Dashboard.Dtos;
using MediatR;

namespace EnterpriseApp.Application.Features.Dashboard.Queries;

/// <summary>
/// Query to get dashboard data for the current user.
/// </summary>
public record GetDashboardQuery : IRequest<DashboardDto>;

/// <summary>
/// Handler for GetDashboardQuery.
/// </summary>
public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    // In a full implementation, this would inject repositories for orders, inventory, etc.

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        // Placeholder implementation - in real app, this would query actual data
        await Task.CompletedTask;

        return new DashboardDto
        {
            Sales = new SalesWidgetDto
            {
                TotalSalesToday = 0,
                TotalSalesThisWeek = 0,
                TotalSalesThisMonth = 0,
                TotalSalesThisYear = 0,
                GrowthPercentage = 0,
                Last7Days = GenerateLast7Days()
            },
            Orders = new OrdersWidgetDto
            {
                TotalOrdersToday = 0,
                PendingOrders = 0,
                ProcessingOrders = 0,
                ShippedOrders = 0,
                DeliveredOrders = 0,
                RecentOrders = []
            },
            Inventory = new InventoryWidgetDto
            {
                TotalProducts = 0,
                LowStockProducts = 0,
                OutOfStockProducts = 0,
                LowStockItems = []
            },
            RecentNotifications = [],
            RecentActivity = []
        };
    }

    private static List<DailySalesDto> GenerateLast7Days()
    {
        var result = new List<DailySalesDto>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        for (var i = 6; i >= 0; i--)
        {
            result.Add(new DailySalesDto
            {
                Date = today.AddDays(-i),
                Amount = 0,
                OrderCount = 0
            });
        }

        return result;
    }
}
