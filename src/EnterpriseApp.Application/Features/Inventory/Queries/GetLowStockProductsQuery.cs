using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Inventory.Dtos;
using EnterpriseApp.Domain.Entities;
using EnterpriseApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Inventory.Queries;

/// <summary>
/// Query to get low stock products.
/// </summary>
public record GetLowStockProductsQuery(
    bool IncludeOutOfStock = true
) : IRequest<IReadOnlyList<LowStockProductDto>>;

/// <summary>
/// Handler for GetLowStockProductsQuery.
/// </summary>
public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, IReadOnlyList<LowStockProductDto>>
{
    private readonly IRepository<Product> _repository;

    public GetLowStockProductsQueryHandler(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LowStockProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary && !i.IsDeleted))
            .Where(p => p.Status == ProductStatus.Active)
            .Where(p => p.StockQuantity <= p.LowStockThreshold);

        if (!request.IncludeOutOfStock)
        {
            query = query.Where(p => p.StockQuantity > 0);
        }

        var products = await query
            .OrderBy(p => p.StockQuantity)
            .Select(p => new LowStockProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                CurrentStock = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                ReorderQuantity = p.LowStockThreshold * 2,
                IsOutOfStock = p.StockQuantity <= 0,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return products;
    }
}
