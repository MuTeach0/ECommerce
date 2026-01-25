namespace ECommerce.API.Contracts.Payments;

public record PaymentResponse(
    Guid Id,
    Guid OrderId,
    string TransactionId,
    decimal Amount,
    string Currency,
    string Status,
    string Provider,
    DateTimeOffset CreatedAtUtc);