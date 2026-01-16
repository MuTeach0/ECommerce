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
[ApiVersion("1.0")]
[Authorize]
public sealed class AddressesController(ISender sender) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Adds a new address to the current user's profile.")]
    [EndpointName("AddAddress")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Add([FromBody] AddAddressRequest request, CancellationToken ct)
    {
        // 1. استخراج الـ CustomerId من الـ Claims (التوكن) لضمان الأمان
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var customerId))
            return Unauthorized();

        // 2. تحويل الـ Request لـ Command مع تمرير الـ CustomerId
        var command = new AddAddressCommand(
            customerId,
            request.Title,
            request.City,
            request.Street,
            request.FullAddress);

        var result = await sender.Send(command, ct);

        // 3. نستخدم Match للرد بالنتيجة
        // ملاحظة: بما إننا معندناش GetById حالياً للعناوين، ممكن نرجع الـ ID مباشرة أو نوجه للـ List
        return result.Match(
            id => CreatedAtAction(nameof(GetMyAddresses), new { version = "1.0" }, id),
            Problem);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Retrieves all addresses associated with the current user.")]
    [EndpointName("GetMyAddresses")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMyAddresses(CancellationToken ct)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        // هنا هنحتاج مستقبلاً GetUserAddressesQuery
         var result = await sender.Send(new GetUserAddressesQuery(userId), ct);
        return result.Match(id => Ok(new { AddressId = id }), Problem);
    }
}