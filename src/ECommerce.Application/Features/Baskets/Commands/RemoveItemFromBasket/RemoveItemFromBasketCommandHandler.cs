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
        var userId = user.Id;
        if (string.IsNullOrEmpty(userId)) return Error.Unauthorized();

        // 1. جلب السلة من Redis
        var basketResult = await basketService.GetBasketAsync(userId);

        // لو السلة مش موجودة أصلاً نرجع خطأ NotFound
        if (basketResult.IsError) return basketResult.Errors;

        var basket = basketResult.Value;

        // 2. حذف المنتج من الـ Entity (الـ Logic جوه الدومين)
        basket.RemoveItem(request.ProductId);

        // 3. تحديث السلة في Redis بعد الحذف
        var result = await basketService.UpdateBasketAsync(basket);

        // 4. الإرجاع (بما إن الـ Command نوعه Result<Unit> بنرجع Success)
        return result.Match<Result<Unit>>(
            _ => Unit.Value,
            errors => errors
        );
    }
}