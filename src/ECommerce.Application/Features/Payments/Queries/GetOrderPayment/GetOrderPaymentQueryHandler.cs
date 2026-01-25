using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Payments.DTOs;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Queries.GetOrderPayment;

public class GetOrderPaymentQueryHandler(IAppDbContext context) 
    : IRequestHandler<GetOrderPaymentQuery, Result<PaymentDTO>>
{
    public async Task<Result<PaymentDTO>> Handle(GetOrderPaymentQuery request, CancellationToken ct)
    {
        var payment = await context.Payments
            .AsNoTracking()
            .Where(p => p.OrderId == request.OrderId)
            .Select(p => new PaymentDTO(
                p.Id, p.OrderId, p.TransactionId, p.Amount, p.Currency, 
                p.Status.ToString(), p.Provider, p.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        return payment is null ? PaymentErrors.PaymentNotFound : payment;
    }
}