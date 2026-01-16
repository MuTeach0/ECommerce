using FluentValidation;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders;

public class GetUserOrdersQueryValidator : AbstractValidator<GetUserOrdersQuery>
{
    public GetUserOrdersQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .NotEqual(Guid.Empty).WithMessage("The provided User ID is invalid.");
    }
}