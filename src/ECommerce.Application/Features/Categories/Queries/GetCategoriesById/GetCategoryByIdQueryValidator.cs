using FluentValidation;

namespace ECommerce.Application.Features.Categories.Queries.GetCategoriesById;

public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}