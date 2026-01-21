using ECommerce.Application.Features.Identity.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<Result<string>> RegisterAsync(string email, string password, string fullName, string phoneNumber);
    Task<bool> AuthorizeAsync(string userId, string? policyName);

    Task<Result<AppUserDTO>> AuthenticateAsync(string email, string password);

    Task<Result<AppUserDTO>> GetUserByIdAsync(string userId);

    Task<string?> GetUserNameAsync(string userId);
}