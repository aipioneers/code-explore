namespace EnterpriseApp.Application.Features.Documents.Dtos;

/// <summary>
/// Document list item DTO.
/// </summary>
public class DocumentDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime DocumentDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public bool HasFile { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Document detail DTO.
/// </summary>
public class DocumentDetailDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime DocumentDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? Notes { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Create document request.
/// </summary>
public class CreateDocumentRequest
{
    public string Type { get; set; } = "Invoice";
    public Guid? OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Generate invoice request.
/// </summary>
public class GenerateInvoiceRequest
{
    public Guid OrderId { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public string? Notes { get; set; }
}
