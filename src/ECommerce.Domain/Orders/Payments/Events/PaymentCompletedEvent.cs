using ECommerce.Domain.Common;

namespace ECommerce.Domain.Orders.Payments.Events;

public sealed record PaymentCompletedEvent(Guid PaymentId, Guid OrderId) : DomainEvent;