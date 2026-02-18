using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Features.Products.Commands;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for category management operations.
/// </summary>
[Authorize]
public class CategoriesController : ApiController
{
    /// <summary>
    /// Gets all categories.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool flatList = false)
    {
        var query = new GetCategoriesQuery(includeInactive, flatList);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a category by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id)
    {
        var query = new GetCategoriesQuery(true, true);
        var categories = await Mediator.Send(query);
        var category = categories.FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new { message = "Category not found." });
        }

        return Ok(category);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.ParentId,
            request.DisplayOrder,
            request.MetaTitle,
            request.MetaDescription);

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetCategory), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Updates a category.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, [FromBody] CreateCategoryRequest request)
    {
        // Implementation would use UpdateCategoryCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Deletes a category (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        // Implementation would use DeleteCategoryCommand
        // Should check if category has products before deletion
        await Task.CompletedTask;
        return NoContent();
    }

    /// <summary>
    /// Gets products in a category.
    /// </summary>
    [HttpGet("{id:guid}/products")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetCategoryProducts(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetProductsQuery(pageNumber, pageSize, CategoryId: id);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }
}
