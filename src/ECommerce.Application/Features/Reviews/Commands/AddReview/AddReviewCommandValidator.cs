using FluentValidation;

namespace ECommerce.Application.Features.Reviews.Commands.AddReview;

public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Stars)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MinimumLength(10).WithMessage("Comment must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
    }
}