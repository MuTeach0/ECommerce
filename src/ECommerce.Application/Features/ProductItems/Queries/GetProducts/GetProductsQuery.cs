using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.ProductItems.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.ProductItems.Queries.GetProducts;

public record GetProductsQuery() : ICachedQuery<Result<List<ProductItemDTO>>>
{
    public string CacheKey => "products-all";
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
    public string[] Tags => ["products-list"];
}