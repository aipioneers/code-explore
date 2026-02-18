using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// Product category entity with hierarchical structure.
/// </summary>
public class Category : BaseEntity
{
    private readonly List<Category> _children = new();
    private readonly List<Product> _products = new();

    /// <summary>
    /// Category name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the category.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Category description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Category image URL.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Parent category ID for hierarchical structure.
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// Parent category navigation property.
    /// </summary>
    public Category? Parent { get; private set; }

    /// <summary>
    /// Child categories.
    /// </summary>
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    /// <summary>
    /// Products in this category.
    /// </summary>
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    /// <summary>
    /// Display order for sorting.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Whether the category is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// SEO meta title.
    /// </summary>
    public string? MetaTitle { get; private set; }

    /// <summary>
    /// SEO meta description.
    /// </summary>
    public string? MetaDescription { get; private set; }

    private Category() { }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    public static Category Create(
        string name,
        string slug,
        string? description = null,
        Guid? parentId = null)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug.ToLowerInvariant().Replace(" ", "-"),
            Description = description,
            ParentId = parentId,
            DisplayOrder = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates category details.
    /// </summary>
    public void Update(
        string name,
        string slug,
        string? description,
        string? imageUrl,
        int displayOrder,
        bool isActive,
        string? metaTitle,
        string? metaDescription)
    {
        Name = name;
        Slug = slug.ToLowerInvariant().Replace(" ", "-");
        Description = description;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Changes the parent category.
    /// </summary>
    public void SetParent(Guid? parentId)
    {
        ParentId = parentId;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the category.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the category.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
    }
}
