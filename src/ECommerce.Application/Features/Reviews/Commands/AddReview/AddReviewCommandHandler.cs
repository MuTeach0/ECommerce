using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;
using ECommerce.Domain.Customers.Reviews;
using ECommerce.Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Reviews.Commands.AddReview;

internal class AddReviewCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<AddReviewCommandHandler> logger) : IRequestHandler<AddReviewCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddReviewCommand request, CancellationToken ct)
    {
        logger.LogInformation("Processing review submission for Product: {ProductId} by User: {UserId}",
            request.ProductId, user.Id);

        if (string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out var userId))
        {
            logger.LogWarning("Review submission failed: Unauthorized user.");
            return Error.Unauthorized();
        }

        // 1. Eligibility Check: Order must be Delivered and contains the Product
        var isEligible = await context.Orders
            .AnyAsync(o => o.CustomerId == userId &&
                           o.Status == OrderStatus.Delivered &&
                           o.OrderItems.Any(i => i.ProductItemId == request.ProductId), ct);

        if (!isEligible)
        {
            logger.LogWarning("User {UserId} is not eligible to review product {ProductId}", userId, request.ProductId);
            return ReviewErrors.NotEligible;
        }

        // 2. Duplicate Check
        var alreadyReviewed = await context.Reviews
            .AnyAsync(r => r.CustomerId == userId && r.ProductItemId == request.ProductId, ct);

        if (alreadyReviewed) return ReviewErrors.AlreadyReviewed;

        // 3. Update Product Stats
        var product = await context.ProductItems.FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);
        if (product is null) return ProductItemErrors.NotFound;

        // 4. Create and Save Review
        var reviewResult = Review.Create(Guid.NewGuid(), request.ProductId, userId, request.Stars, request.Comment);
        if (reviewResult.IsError) return reviewResult.Errors;

        product.UpdateRating(request.Stars); // Updates AverageRating and ReviewsCount
        context.Reviews.Add(reviewResult.Value);

        await context.SaveChangesAsync(ct);

        logger.LogInformation("Review {ReviewId} created successfully for Product {ProductId}",
            reviewResult.Value.Id, request.ProductId);

        return reviewResult.Value.Id;
    }
}