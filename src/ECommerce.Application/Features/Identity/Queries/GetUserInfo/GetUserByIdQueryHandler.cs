using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Identity.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger, IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<AppUserDTO>>
{
    private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<AppUserDTO>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var getUserByIdResult = await _identityService.GetUserByIdAsync(request.UserId!);

        if (getUserByIdResult.IsError)
        {
            _logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}