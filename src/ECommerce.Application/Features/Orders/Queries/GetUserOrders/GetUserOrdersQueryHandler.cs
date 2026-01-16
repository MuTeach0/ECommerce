using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders;

internal class GetUserOrdersQueryHandler(
IAppDbContext context,
ILogger<GetUserOrdersQueryHandler> logger)
: IRequestHandler<GetUserOrdersQuery, Result<IReadOnlyList<UserOrderDTO>>>
{
    public async Task<Result<IReadOnlyList<UserOrderDTO>>> Handle(GetUserOrdersQuery request, CancellationToken ct)
    {
        logger.LogInformation("Fetching orders for user: {UserId}", request.UserId);

        var orders = await context.Orders
            .AsNoTracking() // دايماً استخدم AsNoTracking في الاستعلامات (Read-only) للأداء
            .Where(o => o.CustomerId == request.UserId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new UserOrderDTO(
                o.Id,
                o.CreatedAtUtc,
                o.Status.ToString(),
                o.TotalAmount,
                o.OrderItems.Count
            ))
            .ToListAsync(ct);

        logger.LogInformation("Found {Count} orders for user: {UserId}", orders.Count, request.UserId);

        return orders.AsReadOnly();
    }
}