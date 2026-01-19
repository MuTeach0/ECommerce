using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Baskets;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Baskets.Commands.AddItemToBasket;
public class AddItemToBasketCommandHandler(
    IAppDbContext context,         // For SQL (Product metadata)
    IBasketService basketService, // For Redis (Fast basket storage)
    IUser user)                   // For secure User Identity
    : IRequestHandler<AddItemToBasketCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddItemToBasketCommand request, CancellationToken ct)
    {
        // 1. Validate product existence and fetch details
        var product = await context.ProductItems
            .AsNoTracking()
            .Include(p => p.Category) 
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", "The product you're trying to add doesn't exist.");
        }

        // 2. Resolve User Identity securely
        var userId = user.Id; 
        if (string.IsNullOrEmpty(userId)) return Error.Unauthorized();

        // 3. Initial Stock Check
        if (product.StockQuantity < request.Quantity)
        {
            return Error.Validation("Product.LowStock",
                $"Only {product.StockQuantity} items available in stock.");
        }

        // 4. Retrieve or Create Customer Basket from Redis
        var basketResult = await basketService.GetBasketAsync(userId);
        var basket = basketResult.Match(
            b => b,
            _ => CustomerBasket.Create(userId)
        );

        // 5. Cross-check existing basket quantity + new request against actual stock
        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        int currentInBasket = existingItem?.Quantity ?? 0;

        if (currentInBasket + request.Quantity > product.StockQuantity)
        {
            return Error.Validation("Product.LowStock",
                $"You already have {currentInBasket} in your basket. " +
                $"Only {product.StockQuantity} available in total.");
        }

        // 6. Create BasketItem using DDD patterns
        var itemResult = BasketItem.Create(
            product.Id,
            product.Name,
            product.Price,
            request.Quantity,
            product.Category?.Name ?? "General"
        );

        if (itemResult.IsError) return itemResult.Errors;

        // 7. Execute Domain Logic: Add or Update item in basket entity
        basket.AddOrUpdateItem(itemResult.Value);

        // 8. Persist changes back to Redis
        var updateResult = await basketService.UpdateBasketAsync(basket);

        return updateResult.Match<Result<Guid>>(
            _ => product.Id,
            errors => errors
        );
    }
}