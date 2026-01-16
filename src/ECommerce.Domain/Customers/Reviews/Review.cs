using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Customers.Reviews;

public sealed class Review : AuditableEntity
{
    public Guid ProductItemId { get; private set; }
    public Guid CustomerId { get; private set; }
    public int Stars { get; private set; }
    public string Comment { get; private set; }

    private Review() { } // Required for EF Core

    private Review(Guid id, Guid productItemId, Guid customerId, int stars, string comment)
        : base(id)
    {
        ProductItemId = productItemId;
        CustomerId = customerId;
        Stars = stars;
        Comment = comment;
    }

    public static Result<Review> Create(Guid id, Guid productItemId, Guid customerId, int stars, string comment)
    {
        if (stars < 1 || stars > 5)
            return ReviewErrors.InvalidStars;

        if (string.IsNullOrWhiteSpace(comment) || comment.Length < 10)
            return ReviewErrors.CommentTooShort;

        return new Review(id, productItemId, customerId, stars, comment);
    }
}