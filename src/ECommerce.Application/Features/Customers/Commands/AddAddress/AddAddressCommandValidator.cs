using FluentValidation;

namespace ECommerce.Application.Features.Customers.Commands.AddAddress;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        // التأكد من أن معرف العميل ليس فارغاً
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        // التأكد من وجود عنوان للعنوان (بيت، شغل...)
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Address title is required (e.g., Home, Work).")
            .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.");

        // التأكد من إدخال المدينة
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        // التأكد من إدخال الشارع
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street name is required.");

        // التأكد من إدخال العنوان الكامل وتفاصيله
        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Full address details are required.")
            .MinimumLength(10).WithMessage("Please provide a more detailed address (at least 10 characters).");
    }
}