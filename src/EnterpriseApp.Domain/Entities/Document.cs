using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Document entity for invoices, delivery notes, etc.
/// </summary>
public class Document : BaseEntity
{
    /// <summary>
    /// Document number.
    /// </summary>
    public string DocumentNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Document type.
    /// </summary>
    public DocumentType Type { get; private set; }

    /// <summary>
    /// Document status.
    /// </summary>
    public DocumentStatus Status { get; private set; }

    /// <summary>
    /// Related order ID (if applicable).
    /// </summary>
    public Guid? OrderId { get; private set; }

    /// <summary>
    /// Related order navigation property.
    /// </summary>
    public Order? Order { get; private set; }

    /// <summary>
    /// Related customer ID.
    /// </summary>
    public Guid? CustomerId { get; private set; }

    /// <summary>
    /// Related customer navigation property.
    /// </summary>
    public Customer? Customer { get; private set; }

    /// <summary>
    /// Document date.
    /// </summary>
    public DateTime DocumentDate { get; private set; }

    /// <summary>
    /// Due date (for invoices).
    /// </summary>
    public DateTime? DueDate { get; private set; }

    /// <summary>
    /// Total amount.
    /// </summary>
    public decimal? TotalAmount { get; private set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    public string? Currency { get; private set; }

    /// <summary>
    /// File path/URL.
    /// </summary>
    public string? FilePath { get; private set; }

    /// <summary>
    /// File name.
    /// </summary>
    public string? FileName { get; private set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long? FileSize { get; private set; }

    /// <summary>
    /// MIME type.
    /// </summary>
    public string? MimeType { get; private set; }

    /// <summary>
    /// Notes about the document.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// User who created the document.
    /// </summary>
    public Guid? CreatedByUserId { get; private set; }

    /// <summary>
    /// When the document was sent (if applicable).
    /// </summary>
    public DateTime? SentAt { get; private set; }

    private Document() { }

    /// <summary>
    /// Creates a new document.
    /// </summary>
    public static Document Create(
        string documentNumber,
        DocumentType type,
        DateTime documentDate,
        Guid? orderId = null,
        Guid? customerId = null,
        decimal? totalAmount = null,
        string? currency = null,
        Guid? createdByUserId = null)
    {
        return new Document
        {
            Id = Guid.NewGuid(),
            DocumentNumber = documentNumber,
            Type = type,
            Status = DocumentStatus.Draft,
            DocumentDate = documentDate,
            OrderId = orderId,
            CustomerId = customerId,
            TotalAmount = totalAmount,
            Currency = currency,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Sets the due date.
    /// </summary>
    public void SetDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Attaches a file.
    /// </summary>
    public void AttachFile(string filePath, string fileName, long fileSize, string mimeType)
    {
        FilePath = filePath;
        FileName = fileName;
        FileSize = fileSize;
        MimeType = mimeType;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Finalizes the document (sets status to Final).
    /// </summary>
    public void FinalizeDocument()
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException("Only draft documents can be finalized.");

        Status = DocumentStatus.Final;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks as sent.
    /// </summary>
    public void MarkAsSent()
    {
        Status = DocumentStatus.Sent;
        SentAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Voids the document.
    /// </summary>
    public void Void()
    {
        Status = DocumentStatus.Voided;
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
