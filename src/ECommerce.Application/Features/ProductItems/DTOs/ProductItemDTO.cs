namespace ECommerce.Application.Features.ProductItems.DTOs;

public sealed record ProductItemDTO(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    string SKU,
    Guid CategoryId,
    string? CategoryName,
    decimal AverageRating,
    int ReviewsCount
);