using ECommerce.Domain.Common;

namespace ECommerce.Domain.Orders.Events;

public record OrderCancelledEvent(Guid ProductItemId, int Quantity) : DomainEvent;