namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Stock movement type enumeration.
/// </summary>
public enum StockMovementType
{
    /// <summary>
    /// Stock received from purchase order.
    /// </summary>
    PurchaseReceived = 0,

    /// <summary>
    /// Stock sold via order.
    /// </summary>
    SaleShipped = 1,

    /// <summary>
    /// Stock returned by customer.
    /// </summary>
    CustomerReturn = 2,

    /// <summary>
    /// Stock returned to supplier.
    /// </summary>
    SupplierReturn = 3,

    /// <summary>
    /// Manual adjustment increase.
    /// </summary>
    AdjustmentIn = 4,

    /// <summary>
    /// Manual adjustment decrease.
    /// </summary>
    AdjustmentOut = 5,

    /// <summary>
    /// Stock transfer in from another location.
    /// </summary>
    TransferIn = 6,

    /// <summary>
    /// Stock transfer out to another location.
    /// </summary>
    TransferOut = 7,

    /// <summary>
    /// Inventory count adjustment.
    /// </summary>
    InventoryCount = 8,

    /// <summary>
    /// Stock damaged/written off.
    /// </summary>
    Damaged = 9,

    /// <summary>
    /// Stock lost/missing.
    /// </summary>
    Lost = 10,

    /// <summary>
    /// Initial stock entry.
    /// </summary>
    Initial = 11
}

/// <summary>
/// Inventory count status enumeration.
/// </summary>
public enum InventoryCountStatus
{
    /// <summary>
    /// Count is in draft mode.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Count is in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Count has been completed.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Count has been cancelled.
    /// </summary>
    Cancelled = 3
}

/// <summary>
/// Inventory count type enumeration.
/// </summary>
public enum InventoryCountType
{
    /// <summary>
    /// Full inventory count.
    /// </summary>
    Full = 0,

    /// <summary>
    /// Partial/cycle count.
    /// </summary>
    Partial = 1,

    /// <summary>
    /// Spot check.
    /// </summary>
    SpotCheck = 2
}
