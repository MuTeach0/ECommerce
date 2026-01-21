using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders;

internal class GetUserOrdersQueryHandler(
    IAppDbContext context,
    ILogger<GetUserOrdersQueryHandler> logger,
    IUser userService) // Injected IUser service
    : IRequestHandler<GetUserOrdersQuery, Result<IReadOnlyList<UserOrderDTO>>>
{
    public async Task<Result<IReadOnlyList<UserOrderDTO>>> Handle(GetUserOrdersQuery request, CancellationToken ct)
    {
        // 1. Retrieve and validate the current User ID
        var userIdString = userService.Id;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var customerId))
        {
            logger.LogWarning("Unauthorized attempt to fetch orders. User identity is missing or invalid.");
            return Error.Unauthorized();
        }

        logger.LogInformation("Fetching orders for user: {UserId}", customerId);

        // 2. Query the database using the secure ID from the token
        var orders = await context.Orders
            .AsNoTracking() // Use AsNoTracking for read-only performance optimization
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new UserOrderDTO(
                o.Id,
                o.CreatedAtUtc,
                o.Status.ToString(),
                o.TotalAmount,
                o.OrderItems.Count
            ))
            .ToListAsync(ct);

        logger.LogInformation("Found {Count} orders for user: {UserId}", orders.Count, customerId);

        return orders.AsReadOnly();
    }
}