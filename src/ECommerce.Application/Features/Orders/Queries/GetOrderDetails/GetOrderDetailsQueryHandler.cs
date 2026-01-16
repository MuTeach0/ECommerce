using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQueryHandler(
    IAppDbContext context,
    ILogger<GetOrderDetailsQueryHandler> logger) // حقن الـ Logger هنا
    : IRequestHandler<GetOrderDetailsQuery, Result<OrderDetailsDTO>>
{
    public async Task<Result<OrderDetailsDTO>> Handle(GetOrderDetailsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Fetching details for order ID: {OrderId}", request.OrderId);

        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.Address)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.ProductItem)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            logger.LogWarning("Order not found with ID: {OrderId}", request.OrderId);
            return Error.NotFound("Order.NotFound", "The requested order was not found.");
        }

        var dto = new OrderDetailsDTO(
            order.Id,
            order.CreatedAtUtc,
            order.Status.ToString(),
            order.Address?.FullAddress != null
                ? $"{order.Address.City}, {order.Address.Street}, {order.Address.FullAddress}": "Address not found",
            order.ShippingFee,
            order.TotalAmount,
            [.. order.OrderItems.Select(i => new OrderItemDTO(
                i.ProductItemId,
                i.ProductItem?.Name ?? "Unknown Product",
                i.ProductItem?.Description ?? "No description available",
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice))]
        );

        logger.LogInformation("Successfully retrieved details for order: {OrderId}", request.OrderId);

        return dto;
    }
}