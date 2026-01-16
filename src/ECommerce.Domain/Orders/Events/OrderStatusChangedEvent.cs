using ECommerce.Domain.Common;

namespace ECommerce.Domain.Orders.Events;

public record OrderStatusChangedEvent(Guid OrderId, OrderStatus NewStatus) : DomainEvent;