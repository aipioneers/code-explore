using EnterpriseApp.Application.Features.Inventory.Dtos;
using EnterpriseApp.Application.Features.Inventory.Queries;
using EnterpriseApp.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for inventory management operations.
/// </summary>
[Authorize(Roles = "Administrator,Manager,Lager")]
public class InventoryController : ApiController
{
    /// <summary>
    /// Gets stock movements.
    /// </summary>
    [HttpGet("movements")]
    [ProducesResponseType(typeof(PaginatedResponse<StockMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<StockMovementDto>>> GetStockMovements(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? productId = null,
        [FromQuery] string? type = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetStockMovementsQuery(pageNumber, pageSize, productId, type, fromDate, toDate);
        var result = await Mediator.Send(query);

        return Ok(new PaginatedResponse<StockMovementDto>
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
    /// Creates a stock adjustment.
    /// </summary>
    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StockMovementDto>> CreateAdjustment([FromBody] StockAdjustmentRequest request)
    {
        // Implementation would use CreateStockAdjustmentCommand
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Gets low stock products.
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IReadOnlyList<LowStockProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LowStockProductDto>>> GetLowStockProducts(
        [FromQuery] bool includeOutOfStock = true)
    {
        var query = new GetLowStockProductsQuery(includeOutOfStock);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets inventory counts.
    /// </summary>
    [HttpGet("counts")]
    [ProducesResponseType(typeof(IReadOnlyList<InventoryCountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryCountDto>>> GetInventoryCounts(
        [FromQuery] string? status = null)
    {
        // Implementation would use GetInventoryCountsQuery
        await Task.CompletedTask;
        return Ok(new List<InventoryCountDto>());
    }

    /// <summary>
    /// Gets an inventory count by ID.
    /// </summary>
    [HttpGet("counts/{id:guid}")]
    [ProducesResponseType(typeof(InventoryCountDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryCountDetailDto>> GetInventoryCount(Guid id)
    {
        // Implementation would use GetInventoryCountQuery
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Creates a new inventory count.
    /// </summary>
    [HttpPost("counts")]
    [ProducesResponseType(typeof(InventoryCountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InventoryCountDto>> CreateInventoryCount([FromBody] CreateInventoryCountRequest request)
    {
        // Implementation would use CreateInventoryCountCommand
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Records a count for an item.
    /// </summary>
    [HttpPost("counts/{countId:guid}/items/{itemId:guid}/record")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordCount(Guid countId, Guid itemId, [FromBody] RecordCountRequest request)
    {
        // Implementation would use RecordCountCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Completes an inventory count.
    /// </summary>
    [HttpPost("counts/{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteCount(Guid id)
    {
        // Implementation would use CompleteInventoryCountCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Gets stock movements for a specific product.
    /// </summary>
    [HttpGet("products/{productId:guid}/movements")]
    [ProducesResponseType(typeof(IReadOnlyList<StockMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StockMovementDto>>> GetProductStockMovements(
        Guid productId,
        [FromQuery] int limit = 50)
    {
        var query = new GetStockMovementsQuery(1, limit, productId);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }
}
