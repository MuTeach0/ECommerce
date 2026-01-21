using FluentValidation;

namespace ECommerce.Application.Features.Dashboard.Queries.GetOrderStats;

internal class GetOrderStatsQueryValidator : AbstractValidator<GetOrderStatsQuery>
{
    public GetOrderStatsQueryValidator()
    {
        RuleFor(request => request.Date)
            .NotEmpty()
            .WithErrorCode("Date_Is_Required")
            .WithMessage("Please specify a valid date for the dashboard statistics.");
    }
}