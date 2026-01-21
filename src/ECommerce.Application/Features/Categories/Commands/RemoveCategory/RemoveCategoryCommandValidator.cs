using FluentValidation;

namespace ECommerce.Application.Features.Categories.Commands.RemoveCategory;

public class RemoveCategoryCommandValidator : AbstractValidator<RemoveCategoryCommand>
{
    public RemoveCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required to perform deletion.");
    }
}