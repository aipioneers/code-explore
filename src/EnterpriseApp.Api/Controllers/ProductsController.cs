using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Features.Products.Commands;
using EnterpriseApp.Application.Features.Products.Dtos;
using EnterpriseApp.Application.Features.Products.Queries;
using EnterpriseApp.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for product management operations.
/// </summary>
[Authorize]
public class ProductsController : ApiController
{
    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? status = null,
        [FromQuery] bool? inStock = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false)
    {
        var query = new GetProductsQuery(
            pageNumber, pageSize, search, categoryId, status, inStock, sortBy, sortDesc);
        var result = await Mediator.Send(query);

        return Ok(new PaginatedResponse<ProductDto>
        {
            Items = result.Items,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        });
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> GetProduct(Guid id)
    {
        var query = new GetProductQuery(id);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand(
            request.Sku,
            request.Name,
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

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetProduct), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Updates a product.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        // Implementation would use UpdateProductCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Deletes a product (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        // Implementation would use DeleteProductCommand
        await Task.CompletedTask;
        return NoContent();
    }

    /// <summary>
    /// Updates product stock.
    /// </summary>
    [HttpPut("{id:guid}/stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        // Implementation would use UpdateStockCommand
        await Task.CompletedTask;
        return Ok(new { newQuantity = request.Quantity });
    }

    /// <summary>
    /// Updates product status.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        // Implementation would use UpdateProductStatusCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Searches for products.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        var query = new GetProductsQuery(1, 20, q);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }

    /// <summary>
    /// Gets low stock products.
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetLowStockProducts()
    {
        var query = new GetProductsQuery(1, 100, Status: "Active", SortBy: "stock");
        var result = await Mediator.Send(query);
        var lowStockItems = result.Items.Where(p => p.IsLowStock || p.IsOutOfStock).ToList();
        return Ok(lowStockItems);
    }
}
