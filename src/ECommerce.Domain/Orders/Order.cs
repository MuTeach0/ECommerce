using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers;
using ECommerce.Domain.Orders.Events;
using ECommerce.Domain.Orders.OrderItems;

namespace ECommerce.Domain.Orders;

public sealed class Order : AuditableEntity
{
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public OrderStatus Status { get; private set; }
    public decimal ShippingFee { get; private set; }

    // الربط الجديد بالعنوان
    public Guid AddressId { get; private set; }
    public Address? Address { get; private set; }

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // الإجمالي يحسب تلقائياً من العناصر + مصاريف الشحن
    public decimal TotalAmount => _orderItems.Sum(x => x.TotalPrice) + ShippingFee;

    private Order() { }

    private Order(Guid id, Guid customerId, Guid addressId, decimal shippingFee)
        : base(id)
    {
        CustomerId = customerId;
        AddressId = addressId;
        ShippingFee = shippingFee;
        Status = OrderStatus.Pending; // الحالة الافتراضية عند الإنشاء
    }

    public static Result<Order> Create(Guid id, Guid customerId, Guid addressId, decimal shippingFee)
    {
        // التحقق من أن العميل اختار عنواناً فعلياً
        if (addressId == Guid.Empty)
            return OrderErrors.AddressRequired;

        if (shippingFee < 0)
            return OrderErrors.ShippingFeeNegative;

        if (customerId == Guid.Empty)
            return OrderErrors.CustomerRequired;

        return new Order(id, customerId, addressId, shippingFee);
    }

    public Result<Success> AddItem(Guid productItemId, int quantity, decimal price, decimal cost)
    {
        if (quantity <= 0)
            return OrderErrors.ItemQuantityInvalid;

        if (price <= 0 || cost <= 0)
            return OrderErrors.PriceInvalid;

        if (productItemId == Guid.Empty)
            return OrderErrors.ProductRequired;

        _orderItems.Add(new OrderItem(Guid.NewGuid(), productItemId, quantity, price, cost));

        // إطلاق حدث لخصم المخزن
        AddDomainEvent(new OrderItemAddedEvent(productItemId, quantity));

        return Result.Success;
    }

    public Result<Updated> UpdateStatus(OrderStatus newStatus)
    {
        // 1. فحص الإلغاء
        if (newStatus == OrderStatus.Cancelled)
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                return OrderErrors.CannotCancelAfterShipping;

            if (Status == OrderStatus.Cancelled) return Result.Updated;

            foreach (var item in _orderItems)
            {
                AddDomainEvent(new OrderCancelledEvent(item.ProductItemId, item.Quantity));
            }
        }

        // 2. منع الانتقال العكسي للحالات
        if (newStatus == OrderStatus.Processing && Status == OrderStatus.Shipped)
            return OrderErrors.InvalidStatusTransition;

        // 3. فحص الحالات النهائية
        if (Status == OrderStatus.Cancelled || Status == OrderStatus.Delivered)
            return OrderErrors.FinalStateReached;
        if (Status == newStatus) return Result.Updated;
        Status = newStatus;
        AddDomainEvent(new OrderStatusChangedEvent(Id, newStatus));
        return Result.Updated;
    }
}