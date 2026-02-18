using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Product variant entity (e.g., different sizes, colors).
/// </summary>
public class ProductVariant : BaseEntity
{
    /// <summary>
    /// Parent product ID.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Parent product navigation property.
    /// </summary>
    public Product Product { get; private set; } = null!;

    /// <summary>
    /// Unique variant SKU.
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>
    /// Variant name (e.g., "Red - Large").
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Price adjustment from base price (can be negative).
    /// </summary>
    public decimal PriceAdjustment { get; private set; }

    /// <summary>
    /// Stock quantity for this variant.
    /// </summary>
    public int StockQuantity { get; private set; }

    /// <summary>
    /// Barcode specific to this variant.
    /// </summary>
    public string? Barcode { get; private set; }

    /// <summary>
    /// Variant-specific weight.
    /// </summary>
    public decimal? Weight { get; private set; }

    /// <summary>
    /// Variant image URL.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Display order for sorting.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Whether this variant is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    private ProductVariant() { }

    /// <summary>
    /// Creates a new product variant.
    /// </summary>
    public static ProductVariant Create(
        Guid productId,
        string sku,
        string name,
        decimal priceAdjustment,
        int stockQuantity)
    {
        return new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Sku = sku.ToUpperInvariant(),
            Name = name,
            PriceAdjustment = priceAdjustment,
            StockQuantity = stockQuantity,
            DisplayOrder = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates variant details.
    /// </summary>
    public void Update(
        string name,
        decimal priceAdjustment,
        string? barcode,
        decimal? weight,
        string? imageUrl,
        int displayOrder,
        bool isActive)
    {
        Name = name;
        PriceAdjustment = priceAdjustment;
        Barcode = barcode;
        Weight = weight;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the SKU.
    /// </summary>
    public void UpdateSku(string sku)
    {
        Sku = sku.ToUpperInvariant();
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates stock quantity.
    /// </summary>
    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adjusts stock by delta.
    /// </summary>
    public void AdjustStock(int delta)
    {
        StockQuantity += delta;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the variant.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the variant.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
    }
}
