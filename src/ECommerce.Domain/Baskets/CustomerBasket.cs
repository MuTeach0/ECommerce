using System.Text.Json.Serialization;

namespace ECommerce.Domain.Baskets;
public sealed class CustomerBasket
{
    public string Id { get; private set; }

    // حذفنا readonly من هنا لكي يتمكن الـ Serializer من التعامل معها بمرونة أكبر إذا لزم الأمر
    // private List<BasketItem> _items = []; 

    // 1. أضفنا هذا الـ Attribute لإخبار الـ Serializer أن يربط هذه الخاصية بالـ Constructor
    [JsonInclude]
    public List<BasketItem> Items { get; set; } = new();
    public CustomerBasket() { }
    [JsonConstructor]
    // 2. تأكد أن اسم الباراميتر هو "items" تماماً (ليطابق الخاصية Items)
    public CustomerBasket(string id, List<BasketItem>? items = null)
    {
        Id = id;
        Items = items ?? [];
    }

    public static CustomerBasket Create(string id) => new CustomerBasket(id);

    public void AddOrUpdateItem(BasketItem item)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem is null)
        {
            Items.Add(item);
        }
        else
        {
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
        }
    }

    public void RemoveItem(Guid productId) => Items.RemoveAll(i => i.ProductId == productId);
    public void Clear() => Items.Clear();
}