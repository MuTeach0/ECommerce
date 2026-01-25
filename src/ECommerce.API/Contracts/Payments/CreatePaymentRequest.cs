namespace ECommerce.API.Contracts.Payments;

public record CreatePaymentRequest(Guid OrderId);