using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Gets the MediatR sender for dispatching commands and queries.
    /// </summary>
    protected ISender Mediator => _mediator ??=
        HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Returns an OK result with the data.
    /// </summary>
    protected ActionResult<T> OkResult<T>(T data)
    {
        return Ok(data);
    }

    /// <summary>
    /// Returns a Created result with the location and data.
    /// </summary>
    protected ActionResult<T> CreatedResult<T>(string actionName, object routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, data);
    }

    /// <summary>
    /// Returns a No Content result.
    /// </summary>
    protected new ActionResult NoContent()
    {
        return base.NoContent();
    }

    /// <summary>
    /// Returns a Not Found result with an optional message.
    /// </summary>
    protected ActionResult NotFoundResult(string? message = null)
    {
        return NotFound(new { message = message ?? "Resource not found" });
    }

    /// <summary>
    /// Returns a Bad Request result with validation errors.
    /// </summary>
    protected ActionResult BadRequestResult(string message, IDictionary<string, string[]>? errors = null)
    {
        return BadRequest(new { message, errors });
    }
}
