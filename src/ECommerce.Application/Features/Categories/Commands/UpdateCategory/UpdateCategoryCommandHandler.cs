using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(IAppDbContext context, HybridCache cache,
ILogger<UpdateCategoryCommandHandler> logger)
    : IRequestHandler<UpdateCategoryCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        logger.LogInformation("Attempting to update category {CategoryId}", request.Id);
        // 1. جلب القسم الحالي
        var category = await context.Categories.FindAsync([request.Id], ct);
        if (category is null)
        {
            logger.LogWarning("Update failed: Category {CategoryId} not found.", request.Id);
            return Error.NotFound("Category.NotFound", "Category not found.");
        }
      
        // 2. تأكد إن القسم مش بيشاور على نفسه كـ Parent
        if (request.ParentCategoryId == request.Id)
        {
            return Error.Validation("Category.InvalidParent", "A category cannot be its own parent.");
        }

        // 3. تأكد إن الـ Parent الجديد موجود فعلاً
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await context.Categories.AnyAsync(c => c.Id == request.ParentCategoryId, ct);
            if (!parentExists) return Error.NotFound("Category.ParentNotFound", "The specified parent category does not exist.");
        }

        // 4. استدعاء ميثود الـ Update من الـ Entity
        var updateResult = category.Update(
            request.Name, 
            request.Description, 
            request.ImageUrl, 
            request.ParentCategoryId);

        if (updateResult.IsError) return updateResult.Errors;

        await context.SaveChangesAsync(ct);
        
        // 5. الـ Caching Invalidation
        await cache.RemoveByTagAsync("categories", ct);
        await cache.RemoveAsync($"category-{category.Id}", ct);
        logger.LogInformation("Category {CategoryId} updated successfully.", category.Id);
        return Result.Updated;
    }
}