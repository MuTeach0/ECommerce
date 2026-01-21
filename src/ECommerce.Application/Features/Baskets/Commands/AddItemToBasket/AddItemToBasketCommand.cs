using ECommerce.Domain.Common.Results;
using MediatR;

namespace  ECommerce.Application.Features.Baskets.Commands.AddItemToBasket;

public sealed record AddItemToBasketCommand(Guid ProductId, int Quantity) : IRequest<Result<Guid>>;