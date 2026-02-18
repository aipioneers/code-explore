using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Products.Queries;

/// <summary>
/// Query to get all categories (hierarchical structure).
/// </summary>
public record GetCategoriesQuery(
    bool IncludeInactive = false,
    bool FlatList = false
) : IRequest<IReadOnlyList<CategoryDto>>;

/// <summary>
/// Handler for GetCategoriesQuery.
/// </summary>
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Product> _productRepository;

    public GetCategoriesQueryHandler(
        IRepository<Category> categoryRepository,
        IRepository<Product> productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _categoryRepository.AsNoTracking();

        if (!request.IncludeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var categories = await query
            .Include(c => c.Parent)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        // Get product counts per category
        var productCounts = await _productRepository.AsNoTracking()
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count, cancellationToken);

        if (request.FlatList)
        {
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ParentId = c.ParentId,
                ParentName = c.Parent?.Name,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                ProductCount = productCounts.GetValueOrDefault(c.Id, 0)
            }).ToList();
        }

        // Build hierarchical structure
        var rootCategories = categories.Where(c => c.ParentId == null).ToList();
        return BuildHierarchy(rootCategories, categories, productCounts);
    }

    private static List<CategoryDto> BuildHierarchy(
        IEnumerable<Category> currentLevel,
        List<Category> allCategories,
        Dictionary<Guid, int> productCounts)
    {
        var result = new List<CategoryDto>();

        foreach (var category in currentLevel)
        {
            var children = allCategories.Where(c => c.ParentId == category.Id).ToList();

            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentId = category.ParentId,
                ParentName = category.Parent?.Name,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                ProductCount = productCounts.GetValueOrDefault(category.Id, 0),
                Children = BuildHierarchy(children, allCategories, productCounts)
            };

            result.Add(dto);
        }

        return result;
    }
}
