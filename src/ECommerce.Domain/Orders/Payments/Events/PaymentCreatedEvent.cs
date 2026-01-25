using ECommerce.Domain.Common;

namespace ECommerce.Domain.Orders.Payments.Events;

public record PaymentCreatedEvent(Guid PaymentId, Guid OrderId, decimal Amount) : DomainEvent;