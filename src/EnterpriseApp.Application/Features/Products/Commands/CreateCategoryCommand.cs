using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EnterpriseApp.Application.Features.Products.Commands;

/// <summary>
/// Command to create a new category.
/// </summary>
public record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentId,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription
) : IRequest<Result<CategoryDto>>;

/// <summary>
/// Validator for CreateCategoryCommand.
/// </summary>
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be non-negative.");
    }
}

/// <summary>
/// Handler for CreateCategoryCommand.
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        IRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Verify parent exists if provided
        string? parentName = null;
        if (request.ParentId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent == null)
            {
                return Result.Failure<CategoryDto>("Parent category not found.");
            }
            parentName = parent.Name;
        }

        // Generate slug from name
        var slug = GenerateSlug(request.Name);

        // Ensure unique slug
        var slugExists = await _categoryRepository.FirstOrDefaultAsync(
            c => c.Slug == slug,
            cancellationToken);

        if (slugExists != null)
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
        }

        // Create category
        var category = Category.Create(
            request.Name,
            slug,
            request.Description,
            request.ParentId);

        category.Update(
            request.Name,
            slug,
            request.Description,
            null,
            request.DisplayOrder,
            true,
            request.MetaTitle,
            request.MetaDescription);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentName = parentName,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            ProductCount = 0
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
