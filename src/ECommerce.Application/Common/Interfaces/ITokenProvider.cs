using System.Security.Claims;
using ECommerce.Application.Features.Identity;
using ECommerce.Application.Features.Identity.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDTO user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}