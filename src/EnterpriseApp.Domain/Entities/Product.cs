using EnterpriseApp.Domain.Common;
using EnterpriseApp.Domain.Enums;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Product entity representing an item in the catalog.
/// </summary>
public class Product : BaseEntity
{
    private readonly List<ProductVariant> _variants = new();
    private readonly List<ProductImage> _images = new();

    /// <summary>
    /// Unique product SKU.
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>
    /// Product name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Short product description.
    /// </summary>
    public string? ShortDescription { get; private set; }

    /// <summary>
    /// Full product description (can contain HTML).
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Category navigation property.
    /// </summary>
    public Category Category { get; private set; } = null!;

    /// <summary>
    /// Base price before any variations.
    /// </summary>
    public decimal BasePrice { get; private set; }

    /// <summary>
    /// Cost price for margin calculation.
    /// </summary>
    public decimal? CostPrice { get; private set; }

    /// <summary>
    /// Tax rate percentage.
    /// </summary>
    public decimal TaxRate { get; private set; }

    /// <summary>
    /// Product status.
    /// </summary>
    public ProductStatus Status { get; private set; }

    /// <summary>
    /// Whether the product has variants.
    /// </summary>
    public bool HasVariants { get; private set; }

    /// <summary>
    /// Product weight in kilograms.
    /// </summary>
    public decimal? Weight { get; private set; }

    /// <summary>
    /// Product dimensions.
    /// </summary>
    public string? Dimensions { get; private set; }

    /// <summary>
    /// Manufacturer name.
    /// </summary>
    public string? Manufacturer { get; private set; }

    /// <summary>
    /// Manufacturer part number.
    /// </summary>
    public string? ManufacturerPartNumber { get; private set; }

    /// <summary>
    /// Barcode (EAN/UPC).
    /// </summary>
    public string? Barcode { get; private set; }

    /// <summary>
    /// Stock quantity (for products without variants).
    /// </summary>
    public int StockQuantity { get; private set; }

    /// <summary>
    /// Low stock threshold for alerts.
    /// </summary>
    public int LowStockThreshold { get; private set; }

    /// <summary>
    /// Whether backorders are allowed.
    /// </summary>
    public bool AllowBackorder { get; private set; }

    /// <summary>
    /// Product variants (sizes, colors, etc.).
    /// </summary>
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    /// <summary>
    /// Product images.
    /// </summary>
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    /// <summary>
    /// SEO meta title.
    /// </summary>
    public string? MetaTitle { get; private set; }

    /// <summary>
    /// SEO meta description.
    /// </summary>
    public string? MetaDescription { get; private set; }

    /// <summary>
    /// SEO keywords.
    /// </summary>
    public string? MetaKeywords { get; private set; }

    private Product() { }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    public static Product Create(
        string sku,
        string name,
        string slug,
        Guid categoryId,
        decimal basePrice,
        decimal taxRate = 0)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Sku = sku.ToUpperInvariant(),
            Name = name,
            Slug = slug.ToLowerInvariant().Replace(" ", "-"),
            CategoryId = categoryId,
            BasePrice = basePrice,
            TaxRate = taxRate,
            Status = ProductStatus.Draft,
            StockQuantity = 0,
            LowStockThreshold = 5,
            AllowBackorder = false,
            HasVariants = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates product details.
    /// </summary>
    public void Update(
        string name,
        string slug,
        string? shortDescription,
        string? description,
        Guid categoryId,
        decimal basePrice,
        decimal? costPrice,
        decimal taxRate,
        decimal? weight,
        string? dimensions,
        string? manufacturer,
        string? manufacturerPartNumber,
        string? barcode,
        int lowStockThreshold,
        bool allowBackorder,
        string? metaTitle,
        string? metaDescription,
        string? metaKeywords)
    {
        Name = name;
        Slug = slug.ToLowerInvariant().Replace(" ", "-");
        ShortDescription = shortDescription;
        Description = description;
        CategoryId = categoryId;
        BasePrice = basePrice;
        CostPrice = costPrice;
        TaxRate = taxRate;
        Weight = weight;
        Dimensions = dimensions;
        Manufacturer = manufacturer;
        ManufacturerPartNumber = manufacturerPartNumber;
        Barcode = barcode;
        LowStockThreshold = lowStockThreshold;
        AllowBackorder = allowBackorder;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        MetaKeywords = metaKeywords;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the product status.
    /// </summary>
    public void SetStatus(ProductStatus status)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates stock quantity.
    /// </summary>
    public void UpdateStock(int quantity)
    {
        if (HasVariants)
            throw new InvalidOperationException("Cannot update stock directly for products with variants.");

        StockQuantity = quantity;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adjusts stock by delta (positive or negative).
    /// </summary>
    public void AdjustStock(int delta)
    {
        if (HasVariants)
            throw new InvalidOperationException("Cannot adjust stock directly for products with variants.");

        StockQuantity += delta;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a product variant.
    /// </summary>
    public ProductVariant AddVariant(
        string sku,
        string name,
        decimal priceAdjustment,
        int stockQuantity)
    {
        var variant = ProductVariant.Create(Id, sku, name, priceAdjustment, stockQuantity);
        _variants.Add(variant);
        HasVariants = true;
        ModifiedAt = DateTime.UtcNow;
        return variant;
    }

    /// <summary>
    /// Removes a product variant.
    /// </summary>
    public void RemoveVariant(Guid variantId, Guid deletedBy)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant != null)
        {
            variant.Delete(deletedBy);
            ModifiedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Adds a product image.
    /// </summary>
    public ProductImage AddImage(string url, string? altText, int displayOrder)
    {
        var image = ProductImage.Create(Id, url, altText, displayOrder);
        _images.Add(image);
        ModifiedAt = DateTime.UtcNow;
        return image;
    }

    /// <summary>
    /// Removes a product image.
    /// </summary>
    public void RemoveImage(Guid imageId, Guid deletedBy)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image != null)
        {
            image.Delete(deletedBy);
            ModifiedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Gets the total available stock (product or sum of variants).
    /// </summary>
    public int GetAvailableStock()
    {
        if (HasVariants)
        {
            return _variants.Where(v => !v.IsDeleted).Sum(v => v.StockQuantity);
        }
        return StockQuantity;
    }

    /// <summary>
    /// Checks if the product is low on stock.
    /// </summary>
    public bool IsLowStock()
    {
        return GetAvailableStock() <= LowStockThreshold;
    }

    /// <summary>
    /// Checks if the product is out of stock.
    /// </summary>
    public bool IsOutOfStock()
    {
        return GetAvailableStock() <= 0;
    }

    /// <summary>
    /// Gets the price including tax.
    /// </summary>
    public decimal GetPriceWithTax()
    {
        return BasePrice * (1 + TaxRate / 100);
    }

    /// <summary>
    /// Gets the profit margin percentage.
    /// </summary>
    public decimal? GetMarginPercentage()
    {
        if (!CostPrice.HasValue || CostPrice.Value == 0)
            return null;

        return ((BasePrice - CostPrice.Value) / CostPrice.Value) * 100;
    }
}
