using FluentValidation;

namespace ECommerce.Application.Features.ProductItems.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CostPrice).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(x => x.CostPrice)
            .WithMessage("Selling price must be greater than cost price.");
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}