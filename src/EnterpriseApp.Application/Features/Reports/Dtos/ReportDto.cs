namespace EnterpriseApp.Application.Features.Reports.Dtos;

/// <summary>
/// Sales report DTO.
/// </summary>
public class SalesReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalItemsSold { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public IReadOnlyList<SalesByDayDto> SalesByDay { get; set; } = Array.Empty<SalesByDayDto>();
    public IReadOnlyList<TopProductDto> TopProducts { get; set; } = Array.Empty<TopProductDto>();
    public IReadOnlyList<TopCustomerDto> TopCustomers { get; set; } = Array.Empty<TopCustomerDto>();
    public IReadOnlyList<SalesByStatusDto> SalesByStatus { get; set; } = Array.Empty<SalesByStatusDto>();
}

/// <summary>
/// Sales by day DTO.
/// </summary>
public class SalesByDayDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int OrderCount { get; set; }
}

/// <summary>
/// Top product DTO.
/// </summary>
public class TopProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

/// <summary>
/// Top customer DTO.
/// </summary>
public class TopCustomerDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}

/// <summary>
/// Sales by status DTO.
/// </summary>
public class SalesByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Inventory report DTO.
/// </summary>
public class InventoryReportDto
{
    public DateTime GeneratedAt { get; set; }
    public int TotalProducts { get; set; }
    public int TotalActiveProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public IReadOnlyList<InventoryByCategory> ByCategory { get; set; } = Array.Empty<InventoryByCategory>();
    public IReadOnlyList<StockMovementSummary> RecentMovements { get; set; } = Array.Empty<StockMovementSummary>();
}

/// <summary>
/// Inventory by category DTO.
/// </summary>
public class InventoryByCategory
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int TotalStock { get; set; }
    public decimal StockValue { get; set; }
}

/// <summary>
/// Stock movement summary DTO.
/// </summary>
public class StockMovementSummary
{
    public string MovementType { get; set; } = string.Empty;
    public int Count { get; set; }
    public int TotalQuantity { get; set; }
}

/// <summary>
/// Customer report DTO.
/// </summary>
public class CustomerReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageCustomerValue { get; set; }
    public IReadOnlyList<CustomersByStatus> ByStatus { get; set; } = Array.Empty<CustomersByStatus>();
    public IReadOnlyList<CustomerGrowth> Growth { get; set; } = Array.Empty<CustomerGrowth>();
}

/// <summary>
/// Customers by status DTO.
/// </summary>
public class CustomersByStatus
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Customer growth DTO.
/// </summary>
public class CustomerGrowth
{
    public DateTime Month { get; set; }
    public int NewCustomers { get; set; }
    public int TotalCustomers { get; set; }
}

/// <summary>
/// Report request parameters.
/// </summary>
public class ReportRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? GroupBy { get; set; }
    public bool IncludeDetails { get; set; } = true;
}
