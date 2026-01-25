using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler(
    IAppDbContext context,
    IPaymentService paymentService) : IRequestHandler<CreatePaymentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        // 1. Get order details to determine amount
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null) return Error.NotFound("Order.NotFound", "Order not found.");

        // 2. Call PayPal to create the order
        // We use "USD" as a default or you can get it from order
        var payPalResult = await paymentService.CreateOrderAsync(order.TotalAmount, "USD");

        if (payPalResult.IsError) return payPalResult.Errors;

        // 3. Create Payment record in our DB
        var payment = new Payment(
            order.Id, 
            payPalResult.Value, // PayPal Order ID
            order.TotalAmount, 
            "USD", 
            "PayPal");

        context.Payments.Add(payment);
        
        // This will be wrapped in the transaction behavior
        await context.SaveChangesAsync(ct);

        return payPalResult.Value; // Return PayPal ID to frontend to open the approval gate
    }
}