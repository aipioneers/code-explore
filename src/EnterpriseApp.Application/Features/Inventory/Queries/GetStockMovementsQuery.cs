using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Inventory.Dtos;
using EnterpriseApp.Domain.Entities;
using EnterpriseApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Inventory.Queries;

/// <summary>
/// Query to get stock movements.
/// </summary>
public record GetStockMovementsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? ProductId = null,
    string? MovementType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<PagedList<StockMovementDto>>;

/// <summary>
/// Handler for GetStockMovementsQuery.
/// </summary>
public class GetStockMovementsQueryHandler : IRequestHandler<GetStockMovementsQuery, PagedList<StockMovementDto>>
{
    private readonly IRepository<StockMovement> _repository;

    public GetStockMovementsQueryHandler(IRepository<StockMovement> repository)
    {
        _repository = repository;
    }

    public async Task<PagedList<StockMovementDto>> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.AsNoTracking()
            .Include(s => s.Product)
            .Include(s => s.ProductVariant)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(s => s.ProductId == request.ProductId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.MovementType) &&
            Enum.TryParse<StockMovementType>(request.MovementType, true, out var type))
        {
            query = query.Where(s => s.Type == type);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(s => s.CreatedAt <= request.ToDate.Value);
        }

        query = query.OrderByDescending(s => s.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StockMovementDto
            {
                Id = s.Id,
                ProductId = s.ProductId,
                ProductName = s.Product.Name,
                ProductSku = s.Product.Sku,
                ProductVariantId = s.ProductVariantId,
                VariantName = s.ProductVariant != null ? s.ProductVariant.Name : null,
                Type = s.Type.ToString(),
                Quantity = s.Quantity,
                QuantityBefore = s.QuantityBefore,
                QuantityAfter = s.QuantityAfter,
                ReferenceNumber = s.ReferenceNumber,
                Reason = s.Reason,
                Notes = s.Notes,
                UnitCost = s.UnitCost,
                CreatedByUsername = s.CreatedByUsername,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedList<StockMovementDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
