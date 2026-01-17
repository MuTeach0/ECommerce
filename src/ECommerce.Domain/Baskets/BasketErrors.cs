using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Baskets;

public static class BasketErrors
{
    public static readonly Error QuantityInvalid = Error.Validation(
        "Basket.QuantityInvalid",
        "Quantity must be greater than zero.");

    public static readonly Error PriceInvalid = Error.Validation(
        "Basket.PriceInvalid",
        "Product price cannot be negative.");

    public static readonly Error BasketNotFound = Error.NotFound(
        "Basket.NotFound",
        "The requested basket was not found.");
}