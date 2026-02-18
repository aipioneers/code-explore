using EnterpriseApp.Application.Features.Documents.Dtos;
using EnterpriseApp.Application.Features.Documents.Queries;
using EnterpriseApp.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for document management.
/// </summary>
[Authorize]
public class DocumentsController : ApiController
{
    /// <summary>
    /// Gets documents.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<DocumentDto>>> GetDocuments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? orderId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetDocumentsQuery(pageNumber, pageSize, type, status, customerId, orderId, fromDate, toDate);
        var result = await Mediator.Send(query);

        return Ok(new PaginatedResponse<DocumentDto>
        {
            Items = result.Items,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        });
    }

    /// <summary>
    /// Gets a document by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DocumentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentDetailDto>> GetDocument(Guid id)
    {
        // Implementation would use GetDocumentQuery
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Creates a document.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DocumentDto>> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        // Implementation would use CreateDocumentCommand
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Generates an invoice for an order.
    /// </summary>
    [HttpPost("invoices/generate")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DocumentDto>> GenerateInvoice([FromBody] GenerateInvoiceRequest request)
    {
        // Implementation would use GenerateInvoiceCommand
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Downloads a document file.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        // Implementation would retrieve and return the file
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Sends a document via email.
    /// </summary>
    [HttpPost("{id:guid}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendDocument(Guid id, [FromBody] SendDocumentRequest request)
    {
        // Implementation would use SendDocumentCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Voids a document.
    /// </summary>
    [HttpPost("{id:guid}/void")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VoidDocument(Guid id)
    {
        // Implementation would use VoidDocumentCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Gets documents for an order.
    /// </summary>
    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetOrderDocuments(Guid orderId)
    {
        var query = new GetDocumentsQuery(1, 100, OrderId: orderId);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }

    /// <summary>
    /// Gets documents for a customer.
    /// </summary>
    [HttpGet("by-customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetCustomerDocuments(Guid customerId)
    {
        var query = new GetDocumentsQuery(1, 100, CustomerId: customerId);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }
}

/// <summary>
/// Request to send a document.
/// </summary>
public class SendDocumentRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? Message { get; set; }
}
