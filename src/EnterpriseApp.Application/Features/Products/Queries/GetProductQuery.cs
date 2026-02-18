using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Products.Queries;

/// <summary>
/// Query to get a product by ID.
/// </summary>
public record GetProductQuery(Guid Id) : IRequest<Result<ProductDetailDto>>;

/// <summary>
/// Handler for GetProductQuery.
/// </summary>
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDetailDto>>
{
    private readonly IRepository<Product> _productRepository;

    public GetProductQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDetailDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Variants.Where(v => !v.IsDeleted))
            .Include(p => p.Images.Where(i => !i.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            return Result.Failure<ProductDetailDto>("Product not found.");
        }

        var dto = new ProductDetailDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            BasePrice = product.BasePrice,
            CostPrice = product.CostPrice,
            TaxRate = product.TaxRate,
            PriceWithTax = product.GetPriceWithTax(),
            MarginPercentage = product.GetMarginPercentage(),
            Status = product.Status.ToString(),
            HasVariants = product.HasVariants,
            Weight = product.Weight,
            Dimensions = product.Dimensions,
            Manufacturer = product.Manufacturer,
            ManufacturerPartNumber = product.ManufacturerPartNumber,
            Barcode = product.Barcode,
            StockQuantity = product.GetAvailableStock(),
            LowStockThreshold = product.LowStockThreshold,
            AllowBackorder = product.AllowBackorder,
            IsLowStock = product.IsLowStock(),
            IsOutOfStock = product.IsOutOfStock(),
            MetaTitle = product.MetaTitle,
            MetaDescription = product.MetaDescription,
            MetaKeywords = product.MetaKeywords,
            CreatedAt = product.CreatedAt,
            ModifiedAt = product.ModifiedAt,
            Variants = product.Variants
                .OrderBy(v => v.DisplayOrder)
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Sku = v.Sku,
                    Name = v.Name,
                    PriceAdjustment = v.PriceAdjustment,
                    FinalPrice = product.BasePrice + v.PriceAdjustment,
                    StockQuantity = v.StockQuantity,
                    Barcode = v.Barcode,
                    Weight = v.Weight,
                    ImageUrl = v.ImageUrl,
                    DisplayOrder = v.DisplayOrder,
                    IsActive = v.IsActive
                })
                .ToList(),
            Images = product.Images
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    ThumbnailUrl = i.ThumbnailUrl,
                    AltText = i.AltText,
                    Title = i.Title,
                    DisplayOrder = i.DisplayOrder,
                    IsPrimary = i.IsPrimary
                })
                .ToList()
        };

        return Result.Success(dto);
    }
}
