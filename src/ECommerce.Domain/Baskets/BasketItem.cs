using System.Text.Json.Serialization;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Baskets;

public sealed class BasketItem
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public string CategoryName { get; private set; }
    
    [JsonConstructor]
    private BasketItem(Guid productId, string productName, decimal price, int quantity, string categoryName)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
        CategoryName = categoryName;
    }

    public static Result<BasketItem> Create(Guid productId, string productName, decimal price, int quantity, string categoryName)
    {
        if (quantity <= 0) return BasketErrors.QuantityInvalid;
        if (price < 0) return BasketErrors.PriceInvalid;

        return new BasketItem(productId, productName, price, quantity, categoryName);
    }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}