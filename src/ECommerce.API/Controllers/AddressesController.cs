using Asp.Versioning;
using ECommerce.API.Contracts.Address;
using ECommerce.Application.Features.Customers.Commands.AddAddress;
using ECommerce.Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[Route("api/v{version:apiVersion}/addresses")]
[ApiVersion("2.0")]
[Authorize]
public sealed class AddressesController(ISender sender) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Adds a new address to the current user's profile.")]
    [EndpointName("AddAddress")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Add([FromBody] AddAddressRequest request, CancellationToken ct)
    {
        // 1. Extract the UserId from Claims to ensure security (User can only add addresses to themselves)
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var customerId))
           return Unauthorized();

        // 2. Map the request to the command including the extracted customerId
        var command = new AddAddressCommand(
            request.Title,
            request.City,
            request.Street,
            request.FullAddress);

        var result = await sender.Send(command, ct);

        // 3. Return 201 Created with a link to the address list
        return result.Match(
            id => CreatedAtAction(nameof(GetMyAddresses), new { version = "2.0" }, id),
            Problem);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Retrieves all addresses associated with the current user.")]
    [EndpointName("GetMyAddresses")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetMyAddresses(CancellationToken ct)
    {
        //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (!Guid.TryParse(userIdString, out var userId))
        //    return Unauthorized();
        
        // Fetches the list of addresses using the GetUserAddressesQuery
        var result = await sender.Send(new GetUserAddressesQuery(), ct);
        return result.Match(Ok, Problem);
    }
}