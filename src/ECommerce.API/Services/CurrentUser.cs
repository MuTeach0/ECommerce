using System.Security.Claims;
using ECommerce.Application.Common.Interfaces;

namespace ECommerce.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    // يجلب الـ ID الخاص بالمستخدم من الـ Claims الموجودة في الـ Token
    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}