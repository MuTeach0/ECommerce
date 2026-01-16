using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders;

// لاحظ الوراثة من ICachedQuery مع تحديد نوع الـ Result
public record GetUserOrdersQuery(Guid UserId) 
    : ICachedQuery<Result<IReadOnlyList<UserOrderDTO>>>
{
    public string CacheKey => $"user-orders-{UserId}";
    
    // تأكدنا إن الـ Tags والـ Expiration موجودين حسب الـ Interface بتاعك
    public string[] Tags => ["orders", $"user-{UserId}"];
    
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}