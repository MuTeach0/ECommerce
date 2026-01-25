using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Payments.DTOs;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Queries.GetPayment;

public class GetPaymentQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPaymentQuery, Result<PaymentDTO>>
{
    public async Task<Result<PaymentDTO>> Handle(GetPaymentQuery request, CancellationToken ct)
    {
        var payment = await context.Payments
            .AsNoTracking()
            .Where(p => p.Id == request.PaymentId)
            .Select(p => new PaymentDTO(
                p.Id, p.OrderId, p.TransactionId, p.Amount, p.Currency,
                p.Status.ToString(), p.Provider, p.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        return payment is null ? PaymentErrors.PaymentNotFound : payment;
    }
}