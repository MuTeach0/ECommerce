namespace  ECommerce.Application.Features.ProductItems.DTOs;

public sealed record ProductImageDTO(
    Guid Id,
    string ImageUrl,
    bool IsMain
);