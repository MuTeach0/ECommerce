using FluentValidation;

namespace ECommerce.Application.Features.Payments.Commands.CapturePayment;

public class CapturePaymentCommandValidator : AbstractValidator<CapturePaymentCommand>
{
    public CapturePaymentCommandValidator()
    {
        RuleFor(x => x.PayPalOrderId).NotEmpty();
    }
}