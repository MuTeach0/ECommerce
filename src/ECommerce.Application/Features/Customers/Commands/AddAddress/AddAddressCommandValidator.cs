using FluentValidation;

namespace ECommerce.Application.Features.Customers.Commands.AddAddress;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        // Validate the address title (e.g., Home, Work, Office)
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Address title is required (e.g., Home, Work).")
            .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.");

        // Ensure the city is provided
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        // Ensure the street name is provided
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street name is required.");

        // Ensure the full address is detailed enough
        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Full address details are required.")
            .MinimumLength(10).WithMessage("Please provide a more detailed address (at least 10 characters).");
    }
}