using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Product image entity.
/// </summary>
public class ProductImage : BaseEntity
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
    /// Image URL.
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Thumbnail URL.
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Alt text for accessibility.
    /// </summary>
    public string? AltText { get; private set; }

    /// <summary>
    /// Image title.
    /// </summary>
    public string? Title { get; private set; }

    /// <summary>
    /// Display order for sorting.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Whether this is the primary image.
    /// </summary>
    public bool IsPrimary { get; private set; }

    private ProductImage() { }

    /// <summary>
    /// Creates a new product image.
    /// </summary>
    public static ProductImage Create(
        Guid productId,
        string url,
        string? altText,
        int displayOrder)
    {
        return new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Url = url,
            AltText = altText,
            DisplayOrder = displayOrder,
            IsPrimary = displayOrder == 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates image details.
    /// </summary>
    public void Update(
        string? altText,
        string? title,
        int displayOrder)
    {
        AltText = altText;
        Title = title;
        DisplayOrder = displayOrder;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the thumbnail URL.
    /// </summary>
    public void SetThumbnail(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets this image as the primary image.
    /// </summary>
    public void SetAsPrimary()
    {
        IsPrimary = true;
        DisplayOrder = 0;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes primary status from this image.
    /// </summary>
    public void RemovePrimary()
    {
        IsPrimary = false;
        ModifiedAt = DateTime.UtcNow;
    }

}
