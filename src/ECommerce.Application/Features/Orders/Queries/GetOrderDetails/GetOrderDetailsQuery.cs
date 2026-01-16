using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderDetails;

public sealed record GetOrderDetailsQuery(Guid OrderId) : ICachedQuery<Result<OrderDetailsDTO>>
{
    public string CacheKey => $"order-details-{OrderId}";
    public string[] Tags => ["orders", $"order-{OrderId}"];
    public TimeSpan Expiration => TimeSpan.FromHours(1);
}