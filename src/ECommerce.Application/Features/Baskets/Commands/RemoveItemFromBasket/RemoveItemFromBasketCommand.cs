using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Baskets.Commands.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(Guid ProductId) : IRequest<Result<Unit>>;