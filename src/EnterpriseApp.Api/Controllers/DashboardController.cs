using EnterpriseApp.Application.Features.Dashboard.Dtos;
using EnterpriseApp.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for dashboard operations.
/// </summary>
[Authorize]
public class DashboardController : ApiController
{
    /// <summary>
    /// Gets the dashboard data for the current user.
    /// </summary>
    /// <returns>Dashboard data including sales, orders, inventory, and notifications.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var query = new GetDashboardQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets sales widget data.
    /// </summary>
    [HttpGet("sales")]
    [ProducesResponseType(typeof(SalesWidgetDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SalesWidgetDto>> GetSalesWidget()
    {
        var dashboard = await Mediator.Send(new GetDashboardQuery());
        return Ok(dashboard.Sales);
    }

    /// <summary>
    /// Gets orders widget data.
    /// </summary>
    [HttpGet("orders")]
    [ProducesResponseType(typeof(OrdersWidgetDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrdersWidgetDto>> GetOrdersWidget()
    {
        var dashboard = await Mediator.Send(new GetDashboardQuery());
        return Ok(dashboard.Orders);
    }

    /// <summary>
    /// Gets inventory widget data.
    /// </summary>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(InventoryWidgetDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<InventoryWidgetDto>> GetInventoryWidget()
    {
        var dashboard = await Mediator.Send(new GetDashboardQuery());
        return Ok(dashboard.Inventory);
    }
}
