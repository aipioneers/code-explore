using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Documents.Dtos;
using EnterpriseApp.Domain.Entities;
using EnterpriseApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Documents.Queries;

/// <summary>
/// Query to get documents.
/// </summary>
public record GetDocumentsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? DocumentType = null,
    string? Status = null,
    Guid? CustomerId = null,
    Guid? OrderId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<PagedList<DocumentDto>>;

/// <summary>
/// Handler for GetDocumentsQuery.
/// </summary>
public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, PagedList<DocumentDto>>
{
    private readonly IRepository<Document> _repository;

    public GetDocumentsQueryHandler(IRepository<Document> repository)
    {
        _repository = repository;
    }

    public async Task<PagedList<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.AsNoTracking()
            .Include(d => d.Order)
            .Include(d => d.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.DocumentType) &&
            Enum.TryParse<DocumentType>(request.DocumentType, true, out var type))
        {
            query = query.Where(d => d.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<DocumentStatus>(request.Status, true, out var status))
        {
            query = query.Where(d => d.Status == status);
        }

        if (request.CustomerId.HasValue)
        {
            query = query.Where(d => d.CustomerId == request.CustomerId.Value);
        }

        if (request.OrderId.HasValue)
        {
            query = query.Where(d => d.OrderId == request.OrderId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(d => d.DocumentDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(d => d.DocumentDate <= request.ToDate.Value);
        }

        query = query.OrderByDescending(d => d.DocumentDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                DocumentNumber = d.DocumentNumber,
                Type = d.Type.ToString(),
                Status = d.Status.ToString(),
                OrderId = d.OrderId,
                OrderNumber = d.Order != null ? d.Order.OrderNumber : null,
                CustomerId = d.CustomerId,
                CustomerName = d.Customer != null ? (d.Customer.CompanyName ?? d.Customer.FirstName + " " + d.Customer.LastName) : null,
                DocumentDate = d.DocumentDate,
                DueDate = d.DueDate,
                TotalAmount = d.TotalAmount,
                Currency = d.Currency,
                HasFile = d.FilePath != null,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedList<DocumentDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
