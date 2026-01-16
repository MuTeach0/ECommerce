using ECommerce.Domain.Categories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Customers.Items;
public sealed class ProductItem : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }          // سعر البيع للجمهور
    public decimal CostPrice { get; private set; }      // تكلفة الشراء (الأساسية للحسابات)
    public decimal? DiscountPrice { get; private set; }
    public int StockQuantity { get; private set; }
    public string SKU { get; private set; }
    public bool IsActive { get; private set; }
    public decimal AverageRating { get; private set; }
    public int ReviewsCount { get; private set; }
    // Relationships
    public Guid CustomerId { get; private set; } // البائع (Seller)
    public Customer? Customer { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; set; }
    

#pragma warning disable CS8618
    private ProductItem() { }
#pragma warning restore CS8618

    private ProductItem(Guid id, string name, string description, decimal price, decimal costPrice, int stockQuantity, string sku, Guid customerId, Guid categoryId)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        CostPrice = costPrice; // تعيين التكلفة
        StockQuantity = stockQuantity;
        SKU = sku;
        CustomerId = customerId;
        CategoryId = categoryId;
        IsActive = true;
    }

    public static Result<ProductItem> Create(
        Guid id,
        string name,
        string description,
        decimal price,
        decimal costPrice, // أضيفت هنا
        int stockQuantity,
        string sku,
        Guid customerId,
        Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name)) return ProductItemErrors.NameRequired;
        if (price <= 0) return ProductItemErrors.PriceInvalid;
        if (costPrice <= 0) return ProductItemErrors.PriceInvalid; // يمكن إضافة Error خاص للتكلفة
        if (stockQuantity < 0) return ProductItemErrors.StockInvalid;
        if (price < costPrice) return ProductItemErrors.PriceLessThanCost;
        if (string.IsNullOrWhiteSpace(sku)) return ProductItemErrors.SkuRequired;

        return new ProductItem(id, name, description, price, costPrice, stockQuantity, sku, customerId, categoryId);
    }

    public Result<Updated> Update(string name, string description, decimal price, decimal costPrice, int stockQuantity, string sku)
    {
        if (string.IsNullOrWhiteSpace(name)) return ProductItemErrors.NameRequired;
        if (price <= 0) return ProductItemErrors.PriceInvalid;
        if (costPrice <= 0) return ProductItemErrors.PriceInvalid;
        if (stockQuantity < 0) return ProductItemErrors.StockInvalid;
        if (string.IsNullOrWhiteSpace(sku)) return ProductItemErrors.SkuRequired;

        Name = name;
        Description = description;
        Price = price;
        CostPrice = costPrice; // تحديث التكلفة
        StockQuantity = stockQuantity;
        SKU = sku;

        return Result.Updated;
    }

    // ميثودز إدارة المخزن التي سيستخدمها الـ Event Handlers
    public Result<Success> ReduceStock(int quantity)
    {
        if (quantity <= 0) return Error.Validation("Product.InvalidQuantity", "Quantity must be positive.");

        if (StockQuantity < quantity)
            return Error.Validation("Product.LowStock", $"Insufficient stock for {Name}. Available: {StockQuantity}");

        StockQuantity -= quantity;
        return Result.Success;
    }

    public void RestoreStock(int quantity)
    {
        if (quantity > 0)
        {
            StockQuantity += quantity;
        }
    }
    public void UpdateRating(int newStars)
    {
        decimal totalStars = (AverageRating * ReviewsCount) + newStars;
        ReviewsCount++;
        AverageRating = Math.Round(totalStars / ReviewsCount, 1); // تقريب لكسر واحد
    }

    public void UpdateDiscount(decimal? discountPrice) => DiscountPrice = discountPrice;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}