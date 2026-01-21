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
                context.Customers.Where(c => c.Id == r.CustomerId).Select(c => c.Name).FirstOrDefault() ?? "Anonymous"
            ))
            .ToListAsync(ct);

        return reviews.AsReadOnly();
    }
}