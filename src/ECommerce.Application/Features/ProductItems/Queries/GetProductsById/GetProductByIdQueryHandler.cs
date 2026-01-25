using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.ProductItems.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductItems.Queries.GetProductsById;

public class GetProductByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetProductByIdQuery, Result<ProductItemDTO>>
{
    public async Task<Result<ProductItemDTO>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await context.ProductItems
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.Id == request.ProductId)
            .Select(p => new ProductItemDTO(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.DiscountPrice,
                p.StockQuantity,
                p.SKU,
                p.CategoryId,
                p.Category != null ? p.Category.Name : null,
                p.AverageRating,
                p.ReviewsCount,
                p.Images.Select(img => new ProductImageDTO(
                    img.Id,
                    img.ImageUrl,
                    img.IsMain
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", $"Product with ID {request.ProductId} was not found.");
        }

        return product;
    }
}