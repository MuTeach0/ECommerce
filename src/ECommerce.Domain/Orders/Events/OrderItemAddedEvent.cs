using ECommerce.Domain.Common;

namespace ECommerce.Domain.Orders.Events;

public record OrderItemAddedEvent(Guid ProductItemId, int Quantity) : DomainEvent;