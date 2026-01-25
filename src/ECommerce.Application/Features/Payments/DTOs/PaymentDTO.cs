namespace ECommerce.Application.Features.Payments.DTOs;

public record PaymentDTO(
    Guid Id,
    Guid OrderId,
    string TransactionId,
    decimal Amount,
    string Currency,
    string Status,
    string Provider,
    DateTimeOffset CreatedAtUtc);