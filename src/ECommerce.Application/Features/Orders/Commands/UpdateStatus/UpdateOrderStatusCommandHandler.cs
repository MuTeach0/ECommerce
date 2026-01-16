using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Commands.UpdateStatus;

public class UpdateOrderStatusHandler(
    IAppDbContext context,
    HybridCache cacheService,
    ILogger<UpdateOrderStatusHandler> logger)
    : IRequestHandler<UpdateOrderStatusCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        logger.LogInformation("Attempting to update status for order {OrderId} to {NewStatus}",
            request.OrderId, request.NewStatus);

        // 1. جلب الطلب مع العناصر بتاعته (عشان لو فيه Rollback للمخزن)
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found for status update", request.OrderId);
            return Error.NotFound("Order.NotFound", "The requested order was not found.");
        }

        // 2. استخدام ميثود الـ Domain اللي إنت كتبتها (هي اللي فيها فحص الـ Logic)
        var result = order.UpdateStatus(request.NewStatus);

        if (result.IsError)
        {
            logger.LogWarning("Status update failed for order {OrderId}: {@Errors}",
                request.OrderId, result.Errors);
            return result.Errors;
        }

        // 3. حفظ التغييرات (الـ TransactionBehavior هيغلف العملية دي)
        await context.SaveChangesAsync(ct);

        // 4. إبطال الكاش (Invalidation)
        // بنمسح كاش الـ List وكاش الـ Details عشان البيانات تتحدث عند العميل
        await cacheService.RemoveByTagAsync($"order-{order.Id}", ct);
        await cacheService.RemoveByTagAsync($"user-{order.CustomerId}", ct);

        logger.LogInformation("Order {OrderId} status updated successfully to {NewStatus}",
            order.Id, request.NewStatus);

        return Result.Updated;
    }
}