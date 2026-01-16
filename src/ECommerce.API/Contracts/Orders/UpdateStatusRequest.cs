using ECommerce.Domain.Orders;

namespace ECommerce.API.Contracts.Orders;

public sealed record UpdateStatusRequest(OrderStatus Status);