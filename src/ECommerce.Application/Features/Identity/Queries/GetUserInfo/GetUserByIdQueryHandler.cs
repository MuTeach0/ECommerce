using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Identity.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHandler(
    ILogger<GetUserByIdQueryHandler> logger,
    IIdentityService identityService,
    IUser userService) // Injected the Identity Service
    : IRequestHandler<GetUserByIdQuery, Result<AppUserDTO>>
{
    public async Task<Result<AppUserDTO>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        // 1. Retrieve current user ID from our service
        var userId = userService.Id;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Attempted to retrieve user info without a valid Token ID.");
            return Error.Unauthorized();
        }

        // 2. Fetch user details via IdentityService
        var getUserByIdResult = await identityService.GetUserByIdAsync(userId);

        if (getUserByIdResult.IsError)
        {
            logger.LogError("Failed to retrieve user with Id {UserId}. Error: {ErrorDetails}",
                userId,
                getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}