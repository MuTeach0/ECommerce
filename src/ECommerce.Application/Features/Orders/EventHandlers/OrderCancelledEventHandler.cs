using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Orders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.EventHandlers;

public class OrderCancelledEventHandler(
    IAppDbContext context,
    ILogger<OrderCancelledEventHandler> logger)
    : INotificationHandler<OrderCancelledEvent> // MediatR بيفهم إن ده بيسمع للحدث ده
{
    public async Task Handle(OrderCancelledEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Domain Event: Handling Order Cancellation for Product {ProductId}", notification.ProductItemId);

        // 1. جلب المنتج من قاعدة البيانات
        var product = await context.ProductItems
            .FirstOrDefaultAsync(p => p.Id == notification.ProductItemId, ct);

        if (product is null)
        {
            logger.LogError("Critical: Product {ProductId} not found while trying to restore stock for cancelled order!", notification.ProductItemId);
            return;
        }

        // 2. استخدام ميثود الـ Domain اللي إنت كتبتها
        product.RestoreStock(notification.Quantity);

        // 3. حفظ التغيير
        // ملاحظة: لو الـ Domain Event ده طالع من الـ UpdateStatusHandler، 
        // فالـ SaveChanges هتحصل مرة واحدة في الـ Handler الأساسي (أو هنا حسب تصميم الـ Dispatcher عندك)
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Stock restored for product {ProductName}. Added back: {Quantity}", product.Name, notification.Quantity);
    }
}