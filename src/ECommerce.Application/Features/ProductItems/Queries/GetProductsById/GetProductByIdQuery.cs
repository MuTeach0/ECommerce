using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.ProductItems.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Features.ProductItems.Queries.GetProductsById;

public record GetProductByIdQuery(Guid ProductId) : ICachedQuery<Result<ProductItemDTO>>
{
    public string CacheKey => $"product-{ProductId}";
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    public string[] Tags => ["products-list", $"product-details-{ProductId}"];
}