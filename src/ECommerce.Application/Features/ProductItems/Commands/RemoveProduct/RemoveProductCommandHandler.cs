using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.ProductItems.Commands.RemoveProduct;
public class RemoveProductItemHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<RemoveProductItemHandler> logger)
    : IRequestHandler<RemoveProductCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(RemoveProductCommand request, CancellationToken ct)
    {
        // 1. Log محاولة الحذف
        logger.LogInformation("Attempting to delete product with ID: {ProductId}", request.ProductId);

        var product = await context.ProductItems.FindAsync([request.ProductId], ct);

        if (product is null)
        {
            // 2. Log في حالة عدم الوجود (Warning)
            logger.LogWarning("Delete failed: Product with ID {ProductId} was not found.", request.ProductId);
            return Error.NotFound("Product.NotFound", "Product not found.");
        }

        try
        {
            context.ProductItems.Remove(product);
            await context.SaveChangesAsync(ct);

            // 3. Log نجاح العملية قبل مسح الكاش
            logger.LogInformation("Product {ProductName} (ID: {ProductId}) deleted successfully from database.", 
                product.Name, product.Id);

            // مسح الكاش
            await cache.RemoveByTagAsync("products", ct);
            logger.LogDebug("Cache invalidated for tag 'products' after deleting product {ProductId}.", request.ProductId);

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            // 4. Log في حالة حدوث خطأ غير متوقع (Error)
            logger.LogError(ex, "An error occurred while deleting product {ProductId}.", request.ProductId);
            throw; // أو ترجع Error.Failure بناءً على نظام الـ Results عندك
        }
    }
}