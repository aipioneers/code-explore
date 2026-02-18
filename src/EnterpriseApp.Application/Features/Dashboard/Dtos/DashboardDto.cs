namespace EnterpriseApp.Application.Features.Dashboard.Dtos;

/// <summary>
/// Dashboard data transfer object.
/// </summary>
public class DashboardDto
{
    public SalesWidgetDto Sales { get; set; } = new();
    public OrdersWidgetDto Orders { get; set; } = new();
    public InventoryWidgetDto Inventory { get; set; } = new();
    public IReadOnlyList<NotificationDto> RecentNotifications { get; set; } = Array.Empty<NotificationDto>();
    public IReadOnlyList<RecentActivityDto> RecentActivity { get; set; } = Array.Empty<RecentActivityDto>();
}

/// <summary>
/// Sales widget data.
/// </summary>
public class SalesWidgetDto
{
    public decimal TotalSalesToday { get; set; }
    public decimal TotalSalesThisWeek { get; set; }
    public decimal TotalSalesThisMonth { get; set; }
    public decimal TotalSalesThisYear { get; set; }
    public decimal GrowthPercentage { get; set; }
    public IReadOnlyList<DailySalesDto> Last7Days { get; set; } = Array.Empty<DailySalesDto>();
}

/// <summary>
/// Daily sales data point.
/// </summary>
public class DailySalesDto
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public int OrderCount { get; set; }
}

/// <summary>
/// Orders widget data.
/// </summary>
public class OrdersWidgetDto
{
    public int TotalOrdersToday { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public IReadOnlyList<RecentOrderDto> RecentOrders { get; set; } = Array.Empty<RecentOrderDto>();
}

/// <summary>
/// Recent order summary.
/// </summary>
public class RecentOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Inventory widget data.
/// </summary>
public class InventoryWidgetDto
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public IReadOnlyList<LowStockItemDto> LowStockItems { get; set; } = Array.Empty<LowStockItemDto>();
}

/// <summary>
/// Low stock item summary.
/// </summary>
public class LowStockItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
}

/// <summary>
/// Notification summary.
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Recent activity item.
/// </summary>
public class RecentActivityDto
{
    public string Description { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
