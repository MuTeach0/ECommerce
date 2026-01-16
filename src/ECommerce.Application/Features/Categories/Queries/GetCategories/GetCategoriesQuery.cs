using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Categories.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery() : ICachedQuery<Result<List<CategoryDTO>>>
{
    public string CacheKey => "all-categories";

    public string[] Tags => ["categories"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(30);
}