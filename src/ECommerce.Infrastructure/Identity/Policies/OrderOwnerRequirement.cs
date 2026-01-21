using System.Security.Claims;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Identity;
using ECommerce.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity.Policies;

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
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole(nameof(Role.Admin)) || context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var orderIdString = _httpContextAccessor.HttpContext?.Request.RouteValues["OrderId"]?.ToString();

        if (!Guid.TryParse(orderIdString, out var orderId))
        {
            context.Fail();
            return;
        }

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