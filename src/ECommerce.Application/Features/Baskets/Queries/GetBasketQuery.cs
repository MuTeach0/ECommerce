using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Baskets.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Baskets.Queries;

public sealed record GetBasketQuery : IRequest<Result<BasketDTO>>;