using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Orders;

public static class OrderErrors
{
    public static Error AddressRequired =>
        Error.Validation("Order.AddressRequired", "Shipping address is required.");

    public static Error ShippingFeeNegative =>
        Error.Validation("Order.ShippingFeeNegative", "Shipping fee cannot be negative.");

    public static Error CustomerRequired =>
        Error.Validation("Order.CustomerRequired", "A valid customer ID is required.");

    public static Error ProductRequired =>
        Error.Validation("Order.ProductRequired", "Product ID is required.");

    public static Error ItemQuantityInvalid =>
        Error.Validation("Order.ItemQuantityInvalid", "Quantity must be at least 1.");

    public static Error PriceInvalid =>
        Error.Validation("Order.PriceInvalid", "Price or Cost cannot be zero or negative.");

    public static Error CannotCancelAfterShipping =>
        Error.Conflict("Order.CannotCancel", "Order cannot be cancelled because it has already been shipped or delivered.");

    public static Error InvalidStatusTransition =>
        Error.Validation("Order.InvalidStatusTransition", "Cannot move the order back to a previous stage.");

    public static Error FinalStateReached =>
        Error.Validation("Order.FinalStateReached", "Cannot change the status of a completed or cancelled order.");
}