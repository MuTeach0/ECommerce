namespace ECommerce.Application.Features.Orders.DTOs;

public sealed record OrderDetailsDTO(
    Guid Id,
    DateTimeOffset CreatedAtUtc,
    string Status,
    string ShippingAddress,
    decimal ShippingFee,
    decimal TotalAmount,
    List<OrderItemDTO> Items);