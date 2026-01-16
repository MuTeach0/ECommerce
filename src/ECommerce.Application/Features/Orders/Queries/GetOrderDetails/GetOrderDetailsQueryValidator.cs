using FluentValidation;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQueryValidator : AbstractValidator<GetOrderDetailsQuery>
{
    public GetOrderDetailsQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.")
            .NotEqual(Guid.Empty).WithMessage("The provided Order ID is invalid.");
    }
}