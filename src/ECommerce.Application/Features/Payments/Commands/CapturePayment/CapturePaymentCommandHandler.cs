using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders;
using ECommerce.Domain.Orders.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Commands.CapturePayment;

public class CapturePaymentCommandHandler(
    IAppDbContext context,
    IPaymentService paymentService) : IRequestHandler<CapturePaymentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CapturePaymentCommand request, CancellationToken ct)
    {
        var captureResult = await paymentService.CaptureOrderAsync(request.PayPalOrderId);

        if (captureResult.IsError) return captureResult.Errors;

        var payment = await context.Payments
            .FirstOrDefaultAsync(p => p.TransactionId == request.PayPalOrderId, ct);

        if (payment == null) return PaymentErrors.PaymentNotFound;

        payment.MarkAsCompleted();

        var order = await context.Orders.FindAsync(payment.OrderId);
        order?.UpdateStatus(OrderStatus.Processing);

        await context.SaveChangesAsync(ct);
        return true;
    }
}