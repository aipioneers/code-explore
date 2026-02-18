using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Stock movement entity for tracking inventory changes.
/// </summary>
public class StockMovement : BaseEntity
{
    /// <summary>
    /// Product ID.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Product navigation property.
    /// </summary>
    public Product Product { get; private set; } = null!;

    /// <summary>
    /// Product variant ID (if applicable).
    /// </summary>
    public Guid? ProductVariantId { get; private set; }

    /// <summary>
    /// Product variant navigation property.
    /// </summary>
    public ProductVariant? ProductVariant { get; private set; }

    /// <summary>
    /// Movement type.
    /// </summary>
    public StockMovementType Type { get; private set; }

    /// <summary>
    /// Quantity change (positive for in, negative for out).
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Stock quantity before the movement.
    /// </summary>
    public int QuantityBefore { get; private set; }

    /// <summary>
    /// Stock quantity after the movement.
    /// </summary>
    public int QuantityAfter { get; private set; }

    /// <summary>
    /// Reference number (order number, PO number, etc.).
    /// </summary>
    public string? ReferenceNumber { get; private set; }

    /// <summary>
    /// Reference ID (order ID, etc.).
    /// </summary>
    public Guid? ReferenceId { get; private set; }

    /// <summary>
    /// Reason for the movement.
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// Notes about the movement.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Unit cost at time of movement.
    /// </summary>
    public decimal? UnitCost { get; private set; }

    /// <summary>
    /// User who created the movement.
    /// </summary>
    public Guid? CreatedByUserId { get; private set; }

    /// <summary>
    /// Username who created the movement.
    /// </summary>
    public string? CreatedByUsername { get; private set; }

    private StockMovement() { }

    /// <summary>
    /// Creates a new stock movement.
    /// </summary>
    public static StockMovement Create(
        Guid productId,
        StockMovementType type,
        int quantity,
        int quantityBefore,
        Guid? productVariantId = null,
        string? referenceNumber = null,
        Guid? referenceId = null,
        string? reason = null,
        string? notes = null,
        decimal? unitCost = null,
        Guid? createdByUserId = null,
        string? createdByUsername = null)
    {
        return new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductVariantId = productVariantId,
            Type = type,
            Quantity = quantity,
            QuantityBefore = quantityBefore,
            QuantityAfter = quantityBefore + quantity,
            ReferenceNumber = referenceNumber,
            ReferenceId = referenceId,
            Reason = reason,
            Notes = notes,
            UnitCost = unitCost,
            CreatedByUserId = createdByUserId,
            CreatedByUsername = createdByUsername,
            CreatedAt = DateTime.UtcNow
        };
    }
}
