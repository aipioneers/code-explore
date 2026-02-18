namespace EnterpriseApp.Application.Features.Products.Dtos;

/// <summary>
/// Product list item DTO.
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal PriceWithTax { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Product detail DTO with full information.
/// </summary>
public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal PriceWithTax { get; set; }
    public decimal? MarginPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool HasVariants { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? Manufacturer { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? Barcode { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public bool AllowBackorder { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public IReadOnlyList<ProductVariantDto> Variants { get; set; } = Array.Empty<ProductVariantDto>();
    public IReadOnlyList<ProductImageDto> Images { get; set; } = Array.Empty<ProductImageDto>();
}

/// <summary>
/// Product variant DTO.
/// </summary>
public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public decimal FinalPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? Barcode { get; set; }
    public decimal? Weight { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Product image DTO.
/// </summary>
public class ProductImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Category DTO.
/// </summary>
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public IReadOnlyList<CategoryDto> Children { get; set; } = Array.Empty<CategoryDto>();
}

/// <summary>
/// Request to create a product.
/// </summary>
public class CreateProductRequest
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? Manufacturer { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? Barcode { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public bool AllowBackorder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

/// <summary>
/// Request to update a product.
/// </summary>
public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? Manufacturer { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? Barcode { get; set; }
    public int LowStockThreshold { get; set; }
    public bool AllowBackorder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

/// <summary>
/// Request to create a category.
/// </summary>
public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

/// <summary>
/// Request to update stock.
/// </summary>
public class UpdateStockRequest
{
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}
