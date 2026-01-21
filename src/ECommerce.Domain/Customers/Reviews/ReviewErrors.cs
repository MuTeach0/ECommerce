using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Customers.Reviews;

public static class ReviewErrors
{
    public static Error InvalidStars =>
        Error.Validation("Review.InvalidStars", "Rating must be between 1 and 5 stars.");

    public static Error CommentTooShort =>
        Error.Validation("Review.CommentTooShort", "Comment must be at least 10 characters long.");

    public static Error NotEligible =>
        Error.Validation("Review.NotEligible", "You can only review products you have purchased and received.");

    public static Error AlreadyReviewed =>
        Error.Conflict("Review.AlreadyExists", "You have already submitted a review for this product.");
}