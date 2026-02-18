namespace EnterpriseApp.Application.Features.Inventory.Dtos;

/// <summary>
/// Stock movement DTO.
/// </summary>
public class StockMovementDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid? ProductVariantId { get; set; }
    public string? VariantName { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public decimal? UnitCost { get; set; }
    public string? CreatedByUsername { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Inventory count DTO.
/// </summary>
public class InventoryCountDto
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CountType { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalItems { get; set; }
    public int CountedItems { get; set; }
    public int ItemsWithVariance { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Inventory count detail DTO.
/// </summary>
public class InventoryCountDetailDto
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CountType { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<InventoryCountItemDto> Items { get; set; } = Array.Empty<InventoryCountItemDto>();
}

/// <summary>
/// Inventory count item DTO.
/// </summary>
public class InventoryCountItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid? ProductVariantId { get; set; }
    public string? VariantName { get; set; }
    public int ExpectedQuantity { get; set; }
    public int? CountedQuantity { get; set; }
    public int? Variance { get; set; }
    public string? Notes { get; set; }
    public bool IsCounted { get; set; }
}

/// <summary>
/// Low stock product DTO.
/// </summary>
public class LowStockProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int LowStockThreshold { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsOutOfStock { get; set; }
    public string? PrimaryImageUrl { get; set; }
}

/// <summary>
/// Stock adjustment request.
/// </summary>
public class StockAdjustmentRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; } = "AdjustmentIn";
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Create inventory count request.
/// </summary>
public class CreateInventoryCountRequest
{
    public string CountType { get; set; } = "Full";
    public List<Guid>? ProductIds { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Record count request.
/// </summary>
public class RecordCountRequest
{
    public Guid ItemId { get; set; }
    public int CountedQuantity { get; set; }
    public string? Notes { get; set; }
}
