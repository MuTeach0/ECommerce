using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Categories;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(IAppDbContext context, ILogger<CreateCategoryCommandHandler> logger, // حقن الـ Logger
    HybridCache cacheService)
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        logger.LogInformation("Attempting to create a new category with name: {CategoryName}", request.Name);

        var categoryResult = Category.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.ImageUrl,
            request.ParentCategoryId);

        if (categoryResult.IsError)
        {
            logger.LogWarning("Category creation failed for {CategoryName} with errors: {@Errors}",
                request.Name, categoryResult.Errors);
            return categoryResult.Errors;
        }

        context.Categories.Add(categoryResult.Value);
        await context.SaveChangesAsync(ct);

        await cacheService.RemoveByTagAsync("categories", ct);

        logger.LogInformation("Cache invalidated for tag: categories");

        logger.LogInformation("Category {CategoryName} created successfully with ID: {CategoryId}",
            request.Name, categoryResult.Value.Id);
        return categoryResult.Value.Id;
    }
}