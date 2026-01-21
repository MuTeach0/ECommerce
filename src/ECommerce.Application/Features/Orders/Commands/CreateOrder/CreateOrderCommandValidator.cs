using FluentValidation;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        // التحقق من أن معرف العنوان تم إرساله وليس Guid.Empty
        RuleFor(v => v.AddressId)
            .NotEmpty()
            .WithMessage("Please select a shipping address.");
    }
}