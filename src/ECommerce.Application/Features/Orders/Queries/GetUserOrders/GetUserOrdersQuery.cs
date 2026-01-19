using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders;

// The query no longer takes UserId from the outside
public record GetUserOrdersQuery()
    : ICachedQuery<Result<IReadOnlyList<UserOrderDTO>>>
{
    // We will use a property that will be set inside the handler or via a behavior 
    // But for the CacheKey to work, we need the ID. 
    // In Clean Architecture with Caching, it's better to pass the Service or the ID 
    // derived from the service to ensure the Key is unique per user.

    public string? CurrentUserId { get; private set; }

    public void SetUser(string userId) => CurrentUserId = userId;

    public string CacheKey => $"user-orders-{CurrentUserId ?? "anonymous"}";

    public string[] Tags => ["orders", $"user-{CurrentUserId ?? "anonymous"}"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}