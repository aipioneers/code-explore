using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Domain.Entities;
using EnterpriseApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Products.Queries;

/// <summary>
/// Query to get a paginated list of products.
/// </summary>
public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? CategoryId = null,
    string? Status = null,
    bool? InStock = null,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PagedList<ProductDto>>;

/// <summary>
/// Handler for GetProductsQuery.
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedList<ProductDto>>
{
    private readonly IRepository<Product> _productRepository;

    public GetProductsQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _productRepository.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary && !i.IsDeleted))
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Sku.ToLower().Contains(searchTerm) ||
                p.Name.ToLower().Contains(searchTerm) ||
                (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(searchTerm)) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)));
        }

        // Apply category filter
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ProductStatus>(request.Status, true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        // Apply stock filter
        if (request.InStock.HasValue)
        {
            if (request.InStock.Value)
            {
                query = query.Where(p => p.StockQuantity > 0 || p.AllowBackorder);
            }
            else
            {
                query = query.Where(p => p.StockQuantity <= 0 && !p.AllowBackorder);
            }
        }

        // Apply sorting
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "sku" => request.SortDescending
                ? query.OrderByDescending(p => p.Sku)
                : query.OrderBy(p => p.Sku),
            "name" => request.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => request.SortDescending
                ? query.OrderByDescending(p => p.BasePrice)
                : query.OrderBy(p => p.BasePrice),
            "stock" => request.SortDescending
                ? query.OrderByDescending(p => p.StockQuantity)
                : query.OrderBy(p => p.StockQuantity),
            "createdat" => request.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            "category" => request.SortDescending
                ? query.OrderByDescending(p => p.Category.Name)
                : query.OrderBy(p => p.Category.Name),
            _ => request.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Get page
        var products = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                BasePrice = p.BasePrice,
                PriceWithTax = p.BasePrice * (1 + p.TaxRate / 100),
                Status = p.Status.ToString(),
                StockQuantity = p.StockQuantity,
                IsLowStock = p.StockQuantity <= p.LowStockThreshold && p.StockQuantity > 0,
                IsOutOfStock = p.StockQuantity <= 0,
                PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault(),
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedList<ProductDto>(products, totalCount, request.PageNumber, request.PageSize);
    }
}
