using Asp.Versioning;
using ECommerce.API.Contracts.Orders;
using ECommerce.Application.Features.Orders.Commands.CreateOrder;
using ECommerce.Application.Features.Orders.Commands.UpdateStatus;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Application.Features.Orders.Queries.GetOrderDetails;
using ECommerce.Application.Features.Orders.Queries.GetUserOrders;
using ECommerce.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[Route("api/v{version:apiVersion}/orders")]
[ApiVersion("2.0")]
[Authorize]
public sealed class OrdersController(ISender sender) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Creates a new order for the current user.")]
    [EndpointDescription("Converts the current user's basket into an order and clears the basket.")]
    [EndpointName("CreateOrder")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        // تحويل الطلب الخارجي إلى Command
        var command = new CreateOrderCommand(
            request.AddressId);

        var result = await sender.Send(command, ct);

        return result.Match(
            id => CreatedAtAction(nameof(GetById), new { version = "2.0", orderId = id }, id),
            Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserOrderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Retrieves the current user's orders.")]
    [EndpointName("GetMyOrders")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct)
    {
        // استخراج المعرف من التوكن
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await sender.Send(new GetUserOrdersQuery(userId), ct);

        return result.Match(Ok, Problem);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDetailsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves detailed information about a specific order.")]
    [EndpointName("GetOrderById")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderDetailsQuery(orderId), ct);

        return result.Match(Ok, Problem);
    }

    [HttpPatch("{orderId:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates the status of an existing order.")]
    [EndpointDescription("Allows changing order status (e.g., Shipped, Cancelled).")]
    [EndpointName("UpdateOrderStatus")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        var command = new UpdateOrderStatusCommand(orderId, request.Status);
        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }
}