using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Identity.Queries.GenerateTokens;

public record GenerateTokenQuery(
    string Email,
    string Password) : IRequest<Result<TokenResponse>>;