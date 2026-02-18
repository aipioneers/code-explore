using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Order entity representing a customer order.
/// </summary>
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();

    /// <summary>
    /// Unique order number.
    /// </summary>
    public string OrderNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Customer ID.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Customer navigation property.
    /// </summary>
    public Customer Customer { get; private set; } = null!;

    /// <summary>
    /// Order status.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Order date.
    /// </summary>
    public DateTime OrderDate { get; private set; }

    /// <summary>
    /// Expected delivery date.
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; private set; }

    /// <summary>
    /// Actual delivery date.
    /// </summary>
    public DateTime? DeliveredAt { get; private set; }

    /// <summary>
    /// Subtotal before tax and discounts.
    /// </summary>
    public decimal Subtotal { get; private set; }

    /// <summary>
    /// Discount amount.
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Tax amount.
    /// </summary>
    public decimal TaxAmount { get; private set; }

    /// <summary>
    /// Shipping amount.
    /// </summary>
    public decimal ShippingAmount { get; private set; }

    /// <summary>
    /// Total order amount.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    public string Currency { get; private set; } = "USD";

    /// <summary>
    /// Payment status.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }

    /// <summary>
    /// Payment method.
    /// </summary>
    public string? PaymentMethod { get; private set; }

    /// <summary>
    /// Payment reference (transaction ID).
    /// </summary>
    public string? PaymentReference { get; private set; }

    /// <summary>
    /// When payment was received.
    /// </summary>
    public DateTime? PaidAt { get; private set; }

    /// <summary>
    /// Shipping address - Street.
    /// </summary>
    public string ShippingStreet { get; private set; } = string.Empty;

    /// <summary>
    /// Shipping address - City.
    /// </summary>
    public string ShippingCity { get; private set; } = string.Empty;

    /// <summary>
    /// Shipping address - State.
    /// </summary>
    public string? ShippingState { get; private set; }

    /// <summary>
    /// Shipping address - Postal code.
    /// </summary>
    public string ShippingPostalCode { get; private set; } = string.Empty;

    /// <summary>
    /// Shipping address - Country.
    /// </summary>
    public string ShippingCountry { get; private set; } = string.Empty;

    /// <summary>
    /// Billing address - Street.
    /// </summary>
    public string BillingStreet { get; private set; } = string.Empty;

    /// <summary>
    /// Billing address - City.
    /// </summary>
    public string BillingCity { get; private set; } = string.Empty;

    /// <summary>
    /// Billing address - State.
    /// </summary>
    public string? BillingState { get; private set; }

    /// <summary>
    /// Billing address - Postal code.
    /// </summary>
    public string BillingPostalCode { get; private set; } = string.Empty;

    /// <summary>
    /// Billing address - Country.
    /// </summary>
    public string BillingCountry { get; private set; } = string.Empty;

    /// <summary>
    /// Internal notes (not visible to customer).
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Customer notes/instructions.
    /// </summary>
    public string? CustomerNotes { get; private set; }

    /// <summary>
    /// Tracking number for shipment.
    /// </summary>
    public string? TrackingNumber { get; private set; }

    /// <summary>
    /// Shipping carrier.
    /// </summary>
    public string? ShippingCarrier { get; private set; }

    /// <summary>
    /// Order items.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// User who created the order.
    /// </summary>
    public Guid? CreatedByUserId { get; private set; }

    private Order() { }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    public static Order Create(
        string orderNumber,
        Guid customerId,
        Guid? createdByUserId = null)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Pending,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Sets shipping address.
    /// </summary>
    public void SetShippingAddress(
        string street,
        string city,
        string? state,
        string postalCode,
        string country)
    {
        ShippingStreet = street;
        ShippingCity = city;
        ShippingState = state;
        ShippingPostalCode = postalCode;
        ShippingCountry = country;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets billing address.
    /// </summary>
    public void SetBillingAddress(
        string street,
        string city,
        string? state,
        string postalCode,
        string country)
    {
        BillingStreet = street;
        BillingCity = city;
        BillingState = state;
        BillingPostalCode = postalCode;
        BillingCountry = country;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    public OrderItem AddItem(
        Guid productId,
        string productName,
        string productSku,
        decimal unitPrice,
        int quantity,
        decimal taxRate,
        Guid? variantId = null,
        string? variantName = null)
    {
        var item = OrderItem.Create(
            Id,
            productId,
            productName,
            productSku,
            unitPrice,
            quantity,
            taxRate,
            variantId,
            variantName);

        _items.Add(item);
        RecalculateTotals();
        return item;
    }

    /// <summary>
    /// Removes an item from the order.
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotals();
        }
    }

    /// <summary>
    /// Recalculates order totals.
    /// </summary>
    public void RecalculateTotals()
    {
        Subtotal = _items.Sum(i => i.LineTotal);
        TaxAmount = _items.Sum(i => i.TaxAmount);
        TotalAmount = Subtotal - DiscountAmount + TaxAmount + ShippingAmount;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets discount amount.
    /// </summary>
    public void SetDiscount(decimal amount)
    {
        DiscountAmount = amount;
        RecalculateTotals();
    }

    /// <summary>
    /// Sets shipping amount.
    /// </summary>
    public void SetShipping(decimal amount, string? carrier = null)
    {
        ShippingAmount = amount;
        ShippingCarrier = carrier;
        RecalculateTotals();
    }

    /// <summary>
    /// Confirms the order.
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Starts processing the order.
    /// </summary>
    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be processed.");

        Status = OrderStatus.Processing;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the order as shipped.
    /// </summary>
    public void Ship(string trackingNumber, string? carrier = null)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped.");

        Status = OrderStatus.Shipped;
        TrackingNumber = trackingNumber;
        if (carrier != null) ShippingCarrier = carrier;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the order as delivered.
    /// </summary>
    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be marked as delivered.");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the order.
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel delivered or already cancelled orders.");

        Status = OrderStatus.Cancelled;
        if (!string.IsNullOrEmpty(reason))
        {
            Notes = string.IsNullOrEmpty(Notes)
                ? $"Cancelled: {reason}"
                : $"{Notes}\nCancelled: {reason}";
        }
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records payment.
    /// </summary>
    public void RecordPayment(string method, string? reference = null)
    {
        PaymentStatus = PaymentStatus.Paid;
        PaymentMethod = method;
        PaymentReference = reference;
        PaidAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks payment as failed.
    /// </summary>
    public void FailPayment(string? reference = null)
    {
        PaymentStatus = PaymentStatus.Failed;
        PaymentReference = reference;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Refunds the order.
    /// </summary>
    public void Refund()
    {
        PaymentStatus = PaymentStatus.Refunded;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets internal notes.
    /// </summary>
    public void SetNotes(string? notes)
    {
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets expected delivery date.
    /// </summary>
    public void SetExpectedDelivery(DateTime? date)
    {
        ExpectedDeliveryDate = date;
        ModifiedAt = DateTime.UtcNow;
    }
}
