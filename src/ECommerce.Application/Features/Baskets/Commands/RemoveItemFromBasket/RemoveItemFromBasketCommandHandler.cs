using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Baskets.Commands.RemoveItemFromBasket;
public class RemoveItemFromBasketCommandHandler(
    IBasketService basketService,
    IUser user) : IRequestHandler<RemoveItemFromBasketCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(RemoveItemFromBasketCommand request, CancellationToken ct)
    {
        // 1. Resolve Identity
        var userId = user.Id;
        if (string.IsNullOrEmpty(userId)) return Error.Unauthorized();

        // 2. Fetch Basket from Redis
        var basketResult = await basketService.GetBasketAsync(userId);

        if (basketResult.IsError) return basketResult.Errors;

        var basket = basketResult.Value;

        // 3. Execute removal logic inside the Domain Entity
        basket.RemoveItem(request.ProductId);

        // 4. Save updated basket state to Redis
        var result = await basketService.UpdateBasketAsync(basket);

        return result.Match<Result<Unit>>(
            _ => Unit.Value,
            errors => errors
        );
    }
}