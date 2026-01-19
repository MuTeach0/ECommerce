using ECommerce.Application.Features.Identity.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Identity.Queries.GetUserInfo;

public sealed record GetUserByIdQuery() : IRequest<Result<AppUserDTO>>;