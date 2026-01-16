using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Baskets.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Baskets.Queries;

public record GetBasketQuery : IRequest<Result<BasketDTO>>;

public class GetBasketHandler(
    IBasketService basketService,
    IUser user) : IRequestHandler<GetBasketQuery, Result<BasketDTO>>
{
    public async Task<Result<BasketDTO>> Handle(GetBasketQuery request, CancellationToken ct)
    {
        var userId = user.Id;
        if (string.IsNullOrEmpty(userId)) return Error.Unauthorized();

        var result = await basketService.GetBasketAsync(userId);

        return result.Match<Result<BasketDTO>>(
            basket => new BasketDTO(
                basket.Id,
                basket.Items.Select(i => new BasketItemDTO(
                    i.ProductId, i.ProductName, i.Price, i.Quantity, i.CategoryName)).ToList(),
                basket.Items.Sum(i => i.Price * i.Quantity) // حساب الإجمالي لحظياً
            ),
            errors => errors // لو السلة مش موجودة هيرجع BasketNotFound
        );
    }
}