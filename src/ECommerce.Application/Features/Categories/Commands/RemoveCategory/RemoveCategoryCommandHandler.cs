using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Categories.Commands.RemoveCategory;

public class RemoveCategoryHandler(IAppDbContext context, HybridCache cache, ILogger<RemoveCategoryHandler> logger) 
    : IRequestHandler<RemoveCategoryCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(RemoveCategoryCommand request, CancellationToken ct)
    {
        logger.LogInformation("Attempting to delete category with ID: {CategoryId}", request.Id);

        var category = await context.Categories.FindAsync([request.Id], ct);
        if (category is null)
        {
            logger.LogWarning("Delete failed: Category {CategoryId} not found.", request.Id);
            return Error.NotFound("Category.NotFound", "Category not found.");
        }

        // الـ Business Rule: التأكد من عدم وجود منتجات تابعة لهذا القسم
        var hasProducts = await context.ProductItems.AnyAsync(p => p.CategoryId == request.Id, ct);
        if (hasProducts)
        {
            logger.LogWarning("Delete rejected: Category {CategoryName} ({CategoryId}) has associated products.",
                        category.Name, request.Id);
            return Error.Conflict("Category.HasProducts", "Cannot delete category because it contains associated products.");
        }

        var hasSubCategories = await context.Categories.AnyAsync(c => c.ParentCategoryId == request.Id, ct);
        if (hasSubCategories)
        {
            logger.LogWarning("Delete rejected: Category {CategoryName} has sub-categories.", category.Name);
            return Error.Conflict("Category.HasSubCategories", "Cannot delete category with sub-categories.");
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Category '{CategoryName}' (ID: {CategoryId}) deleted successfully.", category.Name, request.Id);
        
        await cache.RemoveAsync($"category-{request.Id}", ct);
        await cache.RemoveByTagAsync("categories", ct);
        
        return Result.Deleted;
    }
}