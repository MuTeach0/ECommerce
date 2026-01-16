using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Reviews.DTOs;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Reviews.Queries.GetProductReviews;

internal class GetProductReviewsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetProductReviewsQuery, Result<IReadOnlyList<ProductReviewDTO>>>
{
    public async Task<Result<IReadOnlyList<ProductReviewDTO>>> Handle(GetProductReviewsQuery request, CancellationToken ct)
    {
        // 1. Check if product exists
        var productExists = await context.ProductItems.AnyAsync(p => p.Id == request.ProductId, ct);
        if (!productExists) return ProductItemErrors.NotFound;

        // 2. Fetch reviews
        var reviews = await context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductItemId == request.ProductId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new ProductReviewDTO(
                r.Id,
                r.Stars,
                r.Comment,
                r.CreatedAtUtc,
                "Verified Customer" // You can join with Users table here to get the real name
            ))
            .ToListAsync(ct);

        return reviews.AsReadOnly();
    }
}