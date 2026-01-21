namespace ECommerce.Application.Features.Baskets.DTOs;

public sealed record BasketDTO(
    string Id,
    List<BasketItemDTO> Items,
    decimal TotalPrice);
