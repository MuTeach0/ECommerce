using Asp.Versioning;
using ECommerce.API.Contracts.Baskets;
using ECommerce.Application.Features.Baskets.Commands.AddItemToBasket;
using ECommerce.Application.Features.Baskets.Commands.RemoveItemFromBasket;
using ECommerce.Application.Features.Baskets.DTOs;
using ECommerce.Application.Features.Baskets.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Route("api/v{version:apiVersion}/baskets")]
[ApiVersion("2.0")]
[Authorize]
public class BasketsController(ISender sender) : ApiController
{
    [HttpPost("items")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Adds an item to the shopping basket.")]
    [EndpointDescription("Adds a specific product with the given quantity to the current user's basket in Redis.")]
    [EndpointName("AddItemToBasket")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
    {
        var command = new AddItemToBasketCommand(request.ProductId, request.Quantity);
        var result = await sender.Send(command);

        return result.Match(
            id => Ok(id),
            Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(BasketDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Retrieves the current user's basket.")]
    [EndpointDescription("Returns all items currently in the user's basket along with the total price.")]
    [EndpointName("GetBasket")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Get()
    {
        var result = await sender.Send(new GetBasketQuery());
        
        return result.Match(
            basket => Ok(basket),
            Problem);
    }

    [HttpDelete("items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes an item from the basket.")]
    [EndpointName("RemoveItemFromBasket")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        var result = await sender.Send(new RemoveItemFromBasketCommand(productId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}