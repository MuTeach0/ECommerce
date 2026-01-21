using FluentValidation;

namespace ECommerce.Application.Features.Orders.Commands.UpdateStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid order status.");
    }
}