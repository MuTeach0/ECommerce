namespace ECommerce.Application.Features.Orders.DTOs;

public sealed record OrderItemDTO(
    Guid ProductId,
    string ProductName,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);