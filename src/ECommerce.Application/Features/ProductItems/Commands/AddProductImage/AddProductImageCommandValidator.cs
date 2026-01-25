using FluentValidation;

namespace ECommerce.Application.Features.ProductItems.Commands.AddProductImage;

public class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        // Validate the list itself
        RuleFor(x => x.Files)
            .NotEmpty().WithMessage("At least one image file is required.");

        // Validate each file within the list
        RuleForEach(x => x.Files)
            .ChildRules(file =>
            {
                file.RuleFor(f => f)
                    .NotNull().WithMessage("Image file is required.")
                    .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
                    .Must(f => f.Length <= 2 * 1024 * 1024).WithMessage("Max image size is 2MB.")
                    .Must(f => IsSupportedImage(f.ContentType)).WithMessage("Only JPG, PNG, and WebP are supported.");
            });
    }

    private static bool IsSupportedImage(string contentType)
    {
        var supportedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        return supportedTypes.Contains(contentType.ToLower());
    }
}