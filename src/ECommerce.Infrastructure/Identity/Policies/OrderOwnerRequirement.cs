using System.Security.Claims;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Identity;
using ECommerce.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity.Policies;

// المتطلب: يجب أن يكون المستخدم هو صاحب الطلب أو مدير
public class OrderOwnerRequirement : IAuthorizationRequirement;

public class OrderOwnerHandler(IAppDbContext context,
    IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<OrderOwnerRequirement>
{
    private readonly IAppDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrderOwnerRequirement requirement)
    {
        // 1. استخراج الـ UserId من الـ Token
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        // 2. السماح للمدير (Manager/Admin) بالوصول دائماً
        if (context.User.IsInRole(nameof(Role.Admin)) || context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // 3. استخراج الـ OrderId من الرابط (Route) ديناميكياً
        var orderIdString = _httpContextAccessor.HttpContext?.Request.RouteValues["OrderId"]?.ToString();

        if (!Guid.TryParse(orderIdString, out var orderId))
        {
            // إذا لم يكن الطلب موجود في الـ Route، ربما الطلب في الـ Body (حسب تصميم الـ API)
            context.Fail();
            return;
        }

        // 4. التحقق من قاعدة البيانات: هل هذا الطلب يخص هذا المستخدم؟
        var isOwner = await _context.Orders
            .AnyAsync(o => o.Id == orderId && o.CustomerId == userId);

        if (isOwner)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}