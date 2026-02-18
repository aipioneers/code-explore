using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Inventory count/stocktake entity.
/// </summary>
public class InventoryCount : BaseEntity
{
    private readonly List<InventoryCountItem> _items = new();

    /// <summary>
    /// Count reference number.
    /// </summary>
    public string CountNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Count status.
    /// </summary>
    public InventoryCountStatus Status { get; private set; }

    /// <summary>
    /// Count type.
    /// </summary>
    public InventoryCountType CountType { get; private set; }

    /// <summary>
    /// When the count was started.
    /// </summary>
    public DateTime StartedAt { get; private set; }

    /// <summary>
    /// When the count was completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Notes about the count.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// User who created the count.
    /// </summary>
    public Guid? CreatedByUserId { get; private set; }

    /// <summary>
    /// User who completed the count.
    /// </summary>
    public Guid? CompletedByUserId { get; private set; }

    /// <summary>
    /// Count items.
    /// </summary>
    public IReadOnlyCollection<InventoryCountItem> Items => _items.AsReadOnly();

    private InventoryCount() { }

    /// <summary>
    /// Creates a new inventory count.
    /// </summary>
    public static InventoryCount Create(
        string countNumber,
        InventoryCountType countType,
        Guid? createdByUserId = null)
    {
        return new InventoryCount
        {
            Id = Guid.NewGuid(),
            CountNumber = countNumber,
            Status = InventoryCountStatus.Draft,
            CountType = countType,
            StartedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Adds an item to the count.
    /// </summary>
    public InventoryCountItem AddItem(
        Guid productId,
        int expectedQuantity,
        Guid? productVariantId = null)
    {
        var item = InventoryCountItem.Create(Id, productId, expectedQuantity, productVariantId);
        _items.Add(item);
        ModifiedAt = DateTime.UtcNow;
        return item;
    }

    /// <summary>
    /// Starts the count.
    /// </summary>
    public void Start()
    {
        if (Status != InventoryCountStatus.Draft)
            throw new InvalidOperationException("Only draft counts can be started.");

        Status = InventoryCountStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Completes the count.
    /// </summary>
    public void Complete(Guid? completedByUserId = null)
    {
        if (Status != InventoryCountStatus.InProgress)
            throw new InvalidOperationException("Only in-progress counts can be completed.");

        Status = InventoryCountStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedByUserId = completedByUserId;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the count.
    /// </summary>
    public void Cancel()
    {
        if (Status == InventoryCountStatus.Completed)
            throw new InvalidOperationException("Completed counts cannot be cancelled.");

        Status = InventoryCountStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets notes.
    /// </summary>
    public void SetNotes(string? notes)
    {
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Inventory count item entity.
/// </summary>
public class InventoryCountItem : BaseEntity
{
    /// <summary>
    /// Parent count ID.
    /// </summary>
    public Guid InventoryCountId { get; private set; }

    /// <summary>
    /// Parent count navigation property.
    /// </summary>
    public InventoryCount InventoryCount { get; private set; } = null!;

    /// <summary>
    /// Product ID.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Product navigation property.
    /// </summary>
    public Product Product { get; private set; } = null!;

    /// <summary>
    /// Product variant ID.
    /// </summary>
    public Guid? ProductVariantId { get; private set; }

    /// <summary>
    /// Expected quantity (system quantity).
    /// </summary>
    public int ExpectedQuantity { get; private set; }

    /// <summary>
    /// Counted quantity.
    /// </summary>
    public int? CountedQuantity { get; private set; }

    /// <summary>
    /// Variance (counted - expected).
    /// </summary>
    public int? Variance => CountedQuantity.HasValue ? CountedQuantity.Value - ExpectedQuantity : null;

    /// <summary>
    /// Notes about this item.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Whether this item has been counted.
    /// </summary>
    public bool IsCounted { get; private set; }

    private InventoryCountItem() { }

    /// <summary>
    /// Creates a new inventory count item.
    /// </summary>
    public static InventoryCountItem Create(
        Guid inventoryCountId,
        Guid productId,
        int expectedQuantity,
        Guid? productVariantId = null)
    {
        return new InventoryCountItem
        {
            Id = Guid.NewGuid(),
            InventoryCountId = inventoryCountId,
            ProductId = productId,
            ProductVariantId = productVariantId,
            ExpectedQuantity = expectedQuantity,
            IsCounted = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Records the counted quantity.
    /// </summary>
    public void RecordCount(int countedQuantity, string? notes = null)
    {
        CountedQuantity = countedQuantity;
        Notes = notes;
        IsCounted = true;
        ModifiedAt = DateTime.UtcNow;
    }
}
