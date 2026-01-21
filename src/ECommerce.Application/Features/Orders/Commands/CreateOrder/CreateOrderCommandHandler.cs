using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IAppDbContext context,
    IBasketService basketService,
    IUser user,
    HybridCache cacheService,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var userId = user.Id;
        logger.LogInformation("Attempting to create a new order for user: {UserId}", userId);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var customerId))
        {
            logger.LogWarning("Order creation failed: Unauthorized user.");
            return Error.Unauthorized();
        }

        // 1. جلب السلة
        var basketResult = await basketService.GetBasketAsync(userId);
        if (basketResult.IsError) return basketResult.Errors;

        var basket = basketResult.Value;
        if (!basket.Items.Any())
            return Error.Validation("Order.EmptyBasket", "Your basket is empty.");

        // 2. التحقق من العنوان وجلب بياناته لحساب مصاريف الشحن
        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.CustomerId == customerId, ct);

        if (address is null)
        {
            logger.LogWarning("Order creation failed: Address {AddressId} is invalid for Customer {CustomerId}",
                request.AddressId, customerId);
            return Error.Validation("Order.InvalidAddress", "The selected address is invalid.");
        }

        // 3. حساب مصاريف الشحن بناءً على المدينة (Business Logic)
        // ملاحظة: يمكنك جعل هذه القيم في ملف Config أو جدول في قاعدة البيانات مستقبلاً
        decimal calculatedShippingFee = address.City.ToLower() switch
        {
            "cairo" => 50.0m,
            "giza" => 50.0m,
            "alexandria" => 75.0m,
            _ => 100.0m // باقي المحافظات
        };

        // 4. إنشاء الأوردر بالقيمة المحسوبة داخلياً
        var orderResult = Order.Create(Guid.NewGuid(), customerId, request.AddressId, calculatedShippingFee);
        if (orderResult.IsError) return orderResult.Errors;
        var order = orderResult.Value;

        // 5. إضافة المنتجات وتحديث المخزن
        foreach (var item in basket.Items)
        {
            var product = await context.ProductItems.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);
            if (product is null)
                return Error.NotFound("Product.NotFound", $"Product {item.ProductName} no longer exists.");

            var stockResult = product.ReduceStock(item.Quantity);
            if (stockResult.IsError) return stockResult.Errors;

            order.AddItem(product.Id, item.Quantity, item.Price, product.CostPrice);
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        await basketService.DeleteBasketAsync(userId);
        await cacheService.RemoveByTagAsync($"user-{userId}", ct);

        logger.LogInformation("Order {OrderId} created successfully with Shipping Fee: {Fee}", order.Id, calculatedShippingFee);

        return order.Id;
    }
}