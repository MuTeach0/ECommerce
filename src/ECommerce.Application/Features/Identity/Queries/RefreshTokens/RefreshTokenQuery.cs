using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Identity.Queries.RefreshTokens;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;