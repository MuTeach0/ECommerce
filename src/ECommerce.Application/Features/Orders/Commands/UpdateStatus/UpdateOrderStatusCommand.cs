using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.UpdateStatus;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<Result<Updated>>;