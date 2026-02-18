namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Document type enumeration.
/// </summary>
public enum DocumentType
{
    /// <summary>
    /// Sales invoice.
    /// </summary>
    Invoice = 0,

    /// <summary>
    /// Credit note.
    /// </summary>
    CreditNote = 1,

    /// <summary>
    /// Delivery note.
    /// </summary>
    DeliveryNote = 2,

    /// <summary>
    /// Packing slip.
    /// </summary>
    PackingSlip = 3,

    /// <summary>
    /// Quote/Estimate.
    /// </summary>
    Quote = 4,

    /// <summary>
    /// Purchase order.
    /// </summary>
    PurchaseOrder = 5,

    /// <summary>
    /// Return merchandise authorization.
    /// </summary>
    RMA = 6,

    /// <summary>
    /// Receipt.
    /// </summary>
    Receipt = 7,

    /// <summary>
    /// Pro-forma invoice.
    /// </summary>
    ProformaInvoice = 8,

    /// <summary>
    /// Other document type.
    /// </summary>
    Other = 99
}

/// <summary>
/// Document status enumeration.
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// Document is in draft mode.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Document is finalized.
    /// </summary>
    Final = 1,

    /// <summary>
    /// Document has been sent.
    /// </summary>
    Sent = 2,

    /// <summary>
    /// Document has been paid (for invoices).
    /// </summary>
    Paid = 3,

    /// <summary>
    /// Document has been voided.
    /// </summary>
    Voided = 4
}
