using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.ProductItems.Commands.UpdateProduct;
public class UpdateProductItemHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<UpdateProductItemHandler> logger)
    : IRequestHandler<UpdateProductCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateProductCommand request, CancellationToken ct)
{
    logger.LogInformation("Updating product {ProductId} with SKU {SKU}", request.ProductId, request.SKU);

    var product = await context.ProductItems
        .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

    if (product is null)
    {
        logger.LogWarning("Update failed: Product {ProductId} not found.", request.ProductId);
        return Error.NotFound("Product.NotFound", "The product was not found.");
    }

    var updateResult = product.Update(
        request.Name,
        request.Description,
        request.Price,
        request.CostPrice,
        request.StockQuantity,
        request.SKU);

    if (updateResult.IsError)
    {
        logger.LogWarning("Domain validation failed for product {ProductId}", product.Id);
        return updateResult.Errors;
    }

    await context.SaveChangesAsync(ct);

    // مسح الكاش بالطريقة الصحيحة
    await cache.RemoveByTagAsync("products", ct); // مسح القائمة كاملة
    await cache.RemoveAsync($"product-{product.Id}", ct); // مسح تفاصيل المنتج ده بس

    logger.LogInformation("Product {ProductId} updated and cache invalidated.", product.Id);
    return Result.Updated;
}
}