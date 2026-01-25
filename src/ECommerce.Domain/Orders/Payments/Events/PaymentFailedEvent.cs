using ECommerce.Domain.Common;

namespace ECommerce.Domain.Orders.Payments.Events;

public sealed record PaymentFailedEvent(Guid PaymentId, Guid OrderId, string Reason) : DomainEvent;