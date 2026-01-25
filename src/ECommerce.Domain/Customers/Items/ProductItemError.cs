using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Customers.Items;

public static class ProductItemErrors
{
    public static Error NameRequired =>
        Error.Validation("ProductItem.NameRequired", "Product name is required.");

    public static Error PriceInvalid =>
        Error.Validation("ProductItem.PriceInvalid", "Price must be greater than zero.");

    public static Error StockInvalid =>
        Error.Validation("ProductItem.StockInvalid", "Stock quantity cannot be negative.");

    public static Error SkuRequired =>
        Error.Validation("ProductItem.SkuRequired", "SKU (Stock Keeping Unit) is required.");
    public static Error PriceLessThanCost =>
        Error.Validation("Product.InvalidMargin", "Selling price cannot be less than cost price.");
    public static Error NotFound =>
        Error.NotFound("ProductItem.NotFound", "The specified product was not found.");

    public static Error ImageNotFound => 
        Error.NotFound("ProductImage.NotFound", "The specified image was not found.");

    public static Error MainImageRequired => 
        Error.Validation("ProductImage.MainImageRequired", "Product must have at least one main image.");
}