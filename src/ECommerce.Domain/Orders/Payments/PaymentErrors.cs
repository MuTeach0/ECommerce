using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Orders.Payments;

public static class PaymentErrors
{
    public static Error ProviderError(string message) => 
        Error.Failure("Payment.ProviderError", message);

    public static Error PaymentNotFound = 
        Error.NotFound("Payment.NotFound", "The payment record was not found.");
        
    public static Error AlreadyProcessed = 
        Error.Conflict("Payment.AlreadyProcessed", "This payment has already been processed.");
}