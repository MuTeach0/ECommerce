using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Baskets;

public static class BasketErrors
{
    // نستخدم ميثود Validation لأن الكمية والسعر أخطاء مدخلات
    public static readonly Error QuantityInvalid = Error.Validation(
        "Basket.QuantityInvalid",
        "Quantity must be greater than zero.");

    public static readonly Error PriceInvalid = Error.Validation(
        "Basket.PriceInvalid",
        "Product price cannot be negative.");

    // نستخدم ميثود NotFound لأن السلة غير موجودة في Redis
    public static readonly Error BasketNotFound = Error.NotFound(
        "Basket.NotFound",
        "The requested basket was not found.");
}