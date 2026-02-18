using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Order line item entity.
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>
    /// Parent order ID.
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Parent order navigation property.
    /// </summary>
    public Order Order { get; private set; } = null!;

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
    /// Product name at time of order (denormalized).
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Product SKU at time of order (denormalized).
    /// </summary>
    public string ProductSku { get; private set; } = string.Empty;

    /// <summary>
    /// Variant name if applicable (denormalized).
    /// </summary>
    public string? VariantName { get; private set; }

    /// <summary>
    /// Unit price at time of order.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Quantity ordered.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Discount amount per unit.
    /// </summary>
    public decimal UnitDiscount { get; private set; }

    /// <summary>
    /// Tax rate applied.
    /// </summary>
    public decimal TaxRate { get; private set; }

    /// <summary>
    /// Line total before tax.
    /// </summary>
    public decimal LineTotal { get; private set; }

    /// <summary>
    /// Tax amount for this line.
    /// </summary>
    public decimal TaxAmount { get; private set; }

    /// <summary>
    /// Total including tax.
    /// </summary>
    public decimal LineTotalWithTax { get; private set; }

    /// <summary>
    /// Item notes.
    /// </summary>
    public string? Notes { get; private set; }

    private OrderItem() { }

    /// <summary>
    /// Creates a new order item.
    /// </summary>
    public static OrderItem Create(
        Guid orderId,
        Guid productId,
        string productName,
        string productSku,
        decimal unitPrice,
        int quantity,
        decimal taxRate,
        Guid? variantId = null,
        string? variantName = null)
    {
        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            ProductVariantId = variantId,
            ProductName = productName,
            ProductSku = productSku,
            VariantName = variantName,
            UnitPrice = unitPrice,
            Quantity = quantity,
            TaxRate = taxRate,
            CreatedAt = DateTime.UtcNow
        };

        item.RecalculateTotals();
        return item;
    }

    /// <summary>
    /// Updates quantity.
    /// </summary>
    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        RecalculateTotals();
    }

    /// <summary>
    /// Sets discount per unit.
    /// </summary>
    public void SetDiscount(decimal unitDiscount)
    {
        UnitDiscount = unitDiscount;
        RecalculateTotals();
    }

    /// <summary>
    /// Recalculates line totals.
    /// </summary>
    public void RecalculateTotals()
    {
        var effectivePrice = UnitPrice - UnitDiscount;
        LineTotal = effectivePrice * Quantity;
        TaxAmount = LineTotal * (TaxRate / 100);
        LineTotalWithTax = LineTotal + TaxAmount;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets notes for this item.
    /// </summary>
    public void SetNotes(string? notes)
    {
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
    }
}
