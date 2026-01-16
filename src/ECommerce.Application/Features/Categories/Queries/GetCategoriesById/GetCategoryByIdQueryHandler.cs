using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Categories.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Categories.Queries.GetCategoriesById;

public class GetCategoryByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDTO>>
{
    public async Task<Result<CategoryDTO>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await context.Categories
            .AsNoTracking() // لتحسين الأداء لأنها عملية قراءة فقط
            .Include(c => c.ParentCategory) // لجلب بيانات الأب
            .Where(c => c.Id == request.CategoryId)
            .Select(c => new CategoryDTO(
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                c.ParentCategoryId,
                c.ParentCategory != null ? c.ParentCategory.Name : null // جلب الاسم
            ))
            .FirstOrDefaultAsync(ct);

        if (category is null)
        {
            return Error.NotFound("Category.NotFound", $"Category with ID {request.CategoryId} was not found.");
        }

        return category;
    }
}