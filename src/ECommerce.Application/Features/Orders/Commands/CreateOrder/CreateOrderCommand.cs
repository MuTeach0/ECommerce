using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid AddressId) : IRequest<Result<Guid>>;