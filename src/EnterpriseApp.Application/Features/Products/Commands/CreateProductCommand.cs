using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EnterpriseApp.Application.Features.Products.Commands;

/// <summary>
/// Command to create a new product.
/// </summary>
public record CreateProductCommand(
    string Sku,
    string Name,
    string? ShortDescription,
    string? Description,
    Guid CategoryId,
    decimal BasePrice,
    decimal? CostPrice,
    decimal TaxRate,
    decimal? Weight,
    string? Dimensions,
    string? Manufacturer,
    string? ManufacturerPartNumber,
    string? Barcode,
    int LowStockThreshold,
    bool AllowBackorder,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords
) : IRequest<Result<ProductDto>>;

/// <summary>
/// Validator for CreateProductCommand.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
            .Matches("^[A-Za-z0-9-_]+$").WithMessage("SKU can only contain letters, numbers, hyphens, and underscores.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.ShortDescription));

        RuleFor(x => x.Description)
            .MaximumLength(10000).WithMessage("Description must not exceed 10000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Base price must be non-negative.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Cost price must be non-negative.")
            .When(x => x.CostPrice.HasValue);

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 100).WithMessage("Tax rate must be between 0 and 100.");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be positive.")
            .When(x => x.Weight.HasValue);

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Low stock threshold must be non-negative.");
    }
}

/// <summary>
/// Handler for CreateProductCommand.
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate SKU
        var existingProduct = await _productRepository.FirstOrDefaultAsync(
            p => p.Sku.ToUpper() == request.Sku.ToUpper(),
            cancellationToken);

        if (existingProduct != null)
        {
            return Result.Failure<ProductDto>("A product with this SKU already exists.");
        }

        // Verify category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            return Result.Failure<ProductDto>("Category not found.");
        }

        // Generate slug from name
        var slug = GenerateSlug(request.Name);

        // Ensure unique slug
        var slugExists = await _productRepository.FirstOrDefaultAsync(
            p => p.Slug == slug,
            cancellationToken);

        if (slugExists != null)
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
        }

        // Create product
        var product = Product.Create(
            request.Sku,
            request.Name,
            slug,
            request.CategoryId,
            request.BasePrice,
            request.TaxRate);

        product.Update(
            request.Name,
            slug,
            request.ShortDescription,
            request.Description,
            request.CategoryId,
            request.BasePrice,
            request.CostPrice,
            request.TaxRate,
            request.Weight,
            request.Dimensions,
            request.Manufacturer,
            request.ManufacturerPartNumber,
            request.Barcode,
            request.LowStockThreshold,
            request.AllowBackorder,
            request.MetaTitle,
            request.MetaDescription,
            request.MetaKeywords);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            BasePrice = product.BasePrice,
            PriceWithTax = product.GetPriceWithTax(),
            Status = product.Status.ToString(),
            StockQuantity = product.StockQuantity,
            IsLowStock = product.IsLowStock(),
            IsOutOfStock = product.IsOutOfStock(),
            CreatedAt = product.CreatedAt
        });
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "");
    }
}
