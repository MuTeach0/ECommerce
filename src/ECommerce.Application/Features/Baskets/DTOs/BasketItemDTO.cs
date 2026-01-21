namespace ECommerce.Application.Features.Baskets.DTOs;

public sealed record BasketItemDTO(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    string CategoryName);