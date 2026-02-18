using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Features.Orders.Commands;
using EnterpriseApp.Application.Features.Orders.Dtos;
using EnterpriseApp.Application.Features.Orders.Queries;
using EnterpriseApp.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for order management operations.
/// </summary>
[Authorize]
public class OrdersController : ApiController
{
    /// <summary>
    /// Gets a paginated list of orders.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<OrderDto>>> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = true)
    {
        var query = new GetOrdersQuery(
            pageNumber, pageSize, search, customerId, status, paymentStatus,
            fromDate, toDate, sortBy, sortDesc);
        var result = await Mediator.Send(query);

        return Ok(new PaginatedResponse<OrderDto>
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
    /// Gets an order by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailDto>> GetOrder(Guid id)
    {
        var query = new GetOrderQuery(id);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Items,
            request.ShippingAddress,
            request.BillingAddress,
            request.CustomerNotes,
            request.ShippingAmount,
            request.ShippingCarrier);

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetOrder), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Updates order status.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        // Implementation would use UpdateOrderStatusCommand
        await Task.CompletedTask;
        return Ok(new { status = request.Status });
    }

    /// <summary>
    /// Confirms an order.
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmOrder(Guid id)
    {
        // Implementation would use ConfirmOrderCommand
        await Task.CompletedTask;
        return Ok(new { status = "Confirmed" });
    }

    /// <summary>
    /// Ships an order.
    /// </summary>
    [HttpPost("{id:guid}/ship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShipOrder(Guid id, [FromBody] ShipOrderRequest request)
    {
        // Implementation would use ShipOrderCommand
        await Task.CompletedTask;
        return Ok(new { status = "Shipped", trackingNumber = request.TrackingNumber });
    }

    /// <summary>
    /// Marks an order as delivered.
    /// </summary>
    [HttpPost("{id:guid}/deliver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeliverOrder(Guid id)
    {
        // Implementation would use DeliverOrderCommand
        await Task.CompletedTask;
        return Ok(new { status = "Delivered" });
    }

    /// <summary>
    /// Cancels an order.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequest? request)
    {
        // Implementation would use CancelOrderCommand
        await Task.CompletedTask;
        return Ok(new { status = "Cancelled" });
    }

    /// <summary>
    /// Records payment for an order.
    /// </summary>
    [HttpPost("{id:guid}/payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordPaymentRequest request)
    {
        // Implementation would use RecordPaymentCommand
        await Task.CompletedTask;
        return Ok(new { paymentStatus = "Paid" });
    }

    /// <summary>
    /// Gets orders for a specific customer.
    /// </summary>
    [HttpGet("by-customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetCustomerOrders(Guid customerId)
    {
        var query = new GetOrdersQuery(1, 100, CustomerId: customerId);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }

    /// <summary>
    /// Gets recent orders.
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetRecentOrders([FromQuery] int count = 10)
    {
        var query = new GetOrdersQuery(1, count, SortDescending: true);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }
}

/// <summary>
/// Request to ship an order.
/// </summary>
public class ShipOrderRequest
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string? Carrier { get; set; }
}

/// <summary>
/// Request to cancel an order.
/// </summary>
public class CancelOrderRequest
{
    public string? Reason { get; set; }
}
