namespace ECommerce.Domain.Orders.Payments;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded,
    Canceled
}