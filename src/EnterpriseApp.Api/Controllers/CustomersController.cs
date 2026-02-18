using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Features.Customers.Commands;
using EnterpriseApp.Application.Features.Customers.Dtos;
using EnterpriseApp.Application.Features.Customers.Queries;
using EnterpriseApp.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers;

/// <summary>
/// Controller for customer management operations.
/// </summary>
[Authorize]
public class CustomersController : ApiController
{
    /// <summary>
    /// Gets a paginated list of customers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<CustomerDto>>> GetCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false)
    {
        var query = new GetCustomersQuery(pageNumber, pageSize, search, status, sortBy, sortDesc);
        var result = await Mediator.Send(query);

        return Ok(new PaginatedResponse<CustomerDto>
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
    /// Gets a customer by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailDto>> GetCustomer(Guid id)
    {
        var query = new GetCustomerQuery(id);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        var command = new CreateCustomerCommand(
            request.CompanyName,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Mobile,
            request.TaxId,
            request.Notes,
            request.CreditLimit,
            request.PaymentTermsDays);

        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Updates a customer.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        // Implementation would use UpdateCustomerCommand
        await Task.CompletedTask;
        return Ok();
    }

    /// <summary>
    /// Deletes a customer (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        // Implementation would use DeleteCustomerCommand
        await Task.CompletedTask;
        return NoContent();
    }

    /// <summary>
    /// Searches for customers.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> SearchCustomers([FromQuery] string q)
    {
        var query = new GetCustomersQuery(1, 20, q);
        var result = await Mediator.Send(query);
        return Ok(result.Items);
    }

    /// <summary>
    /// Adds an address to a customer.
    /// </summary>
    [HttpPost("{id:guid}/addresses")]
    [ProducesResponseType(typeof(CustomerAddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerAddressDto>> AddAddress(Guid id, [FromBody] AddAddressRequest request)
    {
        // Implementation would use AddCustomerAddressCommand
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Removes an address from a customer.
    /// </summary>
    [HttpDelete("{customerId:guid}/addresses/{addressId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAddress(Guid customerId, Guid addressId)
    {
        // Implementation would use RemoveCustomerAddressCommand
        await Task.CompletedTask;
        return NoContent();
    }
}
