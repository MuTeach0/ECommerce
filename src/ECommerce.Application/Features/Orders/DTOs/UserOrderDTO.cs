namespace ECommerce.Application.Features.Orders.DTOs;

public sealed record UserOrderDTO(
    Guid Id,
    DateTimeOffset CreatedAtUtc,
    string Status,
    decimal TotalAmount,
    int ItemsCount);