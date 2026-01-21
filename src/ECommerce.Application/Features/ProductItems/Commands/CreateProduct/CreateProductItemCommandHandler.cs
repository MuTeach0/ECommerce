using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.ProductItems.Commands.CreateProduct;
public class CreateProductItemCommandHandler(
    IAppDbContext context, 
    ILogger<CreateProductItemCommandHandler> logger) // حقن الـ Logger
    : IRequestHandler<CreateProductItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductItemCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating product {ProductName} with SKU {SKU}", request.Name, request.SKU);

        if (await context.ProductItems.AnyAsync(p => p.SKU == request.SKU, ct))
        {
            return Error.Conflict("Product.DuplicateSKU", "A product with this SKU already exists.");
        }

        var result = ProductItem.Create(
            Guid.NewGuid(), request.Name, request.Description,
            request.Price, request.CostPrice, request.StockQuantity,
            request.SKU, request.CategoryId);

        if (result.IsError)
        {
            logger.LogWarning("Validation failed for product {ProductName}", request.Name);
            return result.Errors;
        }

        context.ProductItems.Add(result.Value);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Product {ProductId} created successfully", result.Value.Id);
        return result.Value.Id;
    }
}