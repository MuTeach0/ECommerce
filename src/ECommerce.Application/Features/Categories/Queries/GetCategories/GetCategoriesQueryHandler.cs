using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Categories.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Categories.Queries.GetCategories;
public class GetCategoriesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDTO>>>
{
    public async Task<Result<List<CategoryDTO>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var categories = await context.Categories
            .AsNoTracking() // لتحسين الأداء لأنها عملية قراءة فقط
            .Where(c => c.IsActive)
            .Select(c => new CategoryDTO(
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                c.ParentCategoryId,
                c.ParentCategory != null ? c.ParentCategory.Name : null))
            .ToListAsync(ct);

        return categories;
    }
}