using ECommerce.Domain.Common;
using ECommerce.Domain.Customers.Items;

namespace ECommerce.Domain.Orders.OrderItems;

public sealed class OrderItem : Entity
{
    public Guid ProductItemId { get; private set; }
    public ProductItem? ProductItem { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; } // السعر وقت الشراء
    public decimal CostPrice { get; private set; } // تكلفة المنتج (للمكسب والخسارة)

    private OrderItem() { }

    public OrderItem(Guid id, Guid productItemId, int quantity, decimal unitPrice, decimal costPrice)
        : base(id)
    {
        ProductItemId = productItemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CostPrice = costPrice;
    }

    public decimal TotalPrice => UnitPrice * Quantity;
}