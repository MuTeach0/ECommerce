using ECommerce.Domain.Common;
using ECommerce.Domain.Orders.Payments.Events;

namespace ECommerce.Domain.Orders.Payments;
public class Payment : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public string TransactionId { get; private set; } // PayPal Order ID
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string Provider { get; private set; }

    // Constructor فارغ لـ EF Core
    private Payment() { }

    public Payment(
        Guid orderId, 
        string transactionId, 
        decimal amount, 
        string currency, 
        string provider) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
        Provider = provider;

        // إضافة حدث "إنشاء دفع"
        AddDomainEvent(new PaymentCreatedEvent(Id, OrderId, Amount));
    }

    // ميثود لإكمال الدفع
    public void MarkAsCompleted()
    {
        if (Status != PaymentStatus.Completed)
        {
            Status = PaymentStatus.Completed;
            AddDomainEvent(new PaymentCompletedEvent(Id, OrderId));
        }
    }

    // ميثود لفشل الدفع
    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        AddDomainEvent(new PaymentFailedEvent(Id, OrderId, reason));
    }
}