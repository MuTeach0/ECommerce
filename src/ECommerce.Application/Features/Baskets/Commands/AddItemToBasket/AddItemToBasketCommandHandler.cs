using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Baskets;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Baskets.Commands.AddItemToBasket;

public class AddItemToBasketCommandHandler(
    IAppDbContext context,        // للتعامل مع SQL (المنتجات)
    IBasketService basketService, // للتعامل مع Redis (السلة)
    IUser user)                   // لجلب الـ UserId الخاص بالمستخدم الحالي
    : IRequestHandler<AddItemToBasketCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddItemToBasketCommand request, CancellationToken ct)
    {
        // 1. التأكد من وجود المنتج في قاعدة البيانات وجلب بياناته (الاسم، السعر)
        var product = await context.ProductItems
            .AsNoTracking().Include(p => p.Category) // لو محتاج اسم القسم
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", "The product you're trying to add doesn't exist.");
        }

        // 2. جلب سلة المستخدم الحالية من Redis
        var userId = user.Id; // الـ UserId هو الـ Key بتاعنا في Redis
        if (string.IsNullOrEmpty(userId)) return Error.Unauthorized();
        if (product.StockQuantity < request.Quantity)
        {
            return Error.Validation("Product.LowStock",
                $"Only {product.StockQuantity} items available in stock.");
        }
        var basketResult = await basketService.GetBasketAsync(userId);

        // لو السلة مش موجودة (أول مرة يضيف منتج)، نكريت واحدة جديدة
        var basket = basketResult.Match(
            b => b,
            _ => CustomerBasket.Create(userId)
        );

        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        int currentInBasket = existingItem?.Quantity ?? 0;

        // 4. الفحص الذهبي: (الموجود في السلة + اللي عايز يضيفه) هل يتخطى المخزن؟
        if (currentInBasket + request.Quantity > product.StockQuantity)
        {
            return Error.Validation("Product.LowStock",
                $"You already have {currentInBasket} in basket. " +
                $"Only {product.StockQuantity} available in total.");
        }

        // 3. إنشاء الـ BasketItem (بنمط الـ DDD والـ Result)
        var itemResult = BasketItem.Create(
            product.Id,
            product.Name,
            product.Price,
            request.Quantity,
            product.Category?.Name ?? "General"
        );

        if (itemResult.IsError) return itemResult.Errors;

        // 4. إضافة المنتج للسلة (الـ Logic ده جوه الـ Domain Entity)
        basket.AddOrUpdateItem(itemResult.Value);

        // 5. حفظ التعديلات في Redis
        var updateResult = await basketService.UpdateBasketAsync(basket);

        return updateResult.Match<Result<Guid>>(
            _ => product.Id,
            errors => errors
        );
    }
}
