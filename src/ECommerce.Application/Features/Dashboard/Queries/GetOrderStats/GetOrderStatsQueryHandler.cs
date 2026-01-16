using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Dashboard.DTOs;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Dashboard.Queries.GetOrderStats;

public class GetOrderStatsQueryHandler(IAppDbContext context) :
    IRequestHandler<GetOrderStatsQuery, Result<TodayOrderStatsDTO>>
{
    public async Task<Result<TodayOrderStatsDTO>> Handle(GetOrderStatsQuery request, CancellationToken cancellationToken)
    {
        var start = request.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = request.Date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        // Fetch orders for the specific day
        var query = context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CreatedAtUtc >= start && o.CreatedAtUtc < end);

        var totalOrders = await query.CountAsync(cancellationToken);

        if (totalOrders == 0)
        {
            return new TodayOrderStatsDTO { Date = request.Date };
        }

        var orders = await query.ToListAsync(cancellationToken);

        // Financial Calculations
        var grossRevenue = orders.Sum(o => o.TotalAmount);
        var totalShippingFees = orders.Sum(o => o.ShippingFee);
        var totalProductCost = orders.SelectMany(o => o.OrderItems).Sum(oi => oi.CostPrice * oi.Quantity);
        
        var netProfit = grossRevenue - totalProductCost - totalShippingFees;
        var uniqueCustomers = orders.Select(o => o.CustomerId).Distinct().Count();
        var totalItemsSold = orders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);

        return new TodayOrderStatsDTO
        {
            Date = request.Date,
            TotalOrders = totalOrders,
            Pending = orders.Count(o => o.Status == OrderStatus.Pending),
            Processing = orders.Count(o => o.Status == OrderStatus.Processing),
            Shipped = orders.Count(o => o.Status == OrderStatus.Shipped),
            Delivered = orders.Count(o => o.Status == OrderStatus.Delivered),
            Cancelled = orders.Count(o => o.Status == OrderStatus.Cancelled),
            Returned = orders.Count(o => o.Status == OrderStatus.Returned),
            
            GrossRevenue = grossRevenue,
            TotalProductCost = totalProductCost,
            TotalShippingFees = totalShippingFees,
            NetProfit = netProfit,
            ProfitMargin = grossRevenue > 0 ? (netProfit / grossRevenue) * 100 : 0,
            
            UniqueCustomers = uniqueCustomers,
            TotalItemsSold = totalItemsSold,
            
            AverageOrderValue = totalOrders > 0 ? grossRevenue / totalOrders : 0,
            ReturnRate = totalOrders > 0 ? ((decimal)orders.Count(o => o.Status == OrderStatus.Returned) / totalOrders) * 100 : 0,
            CancellationRate = totalOrders > 0 ? ((decimal)orders.Count(o => o.Status == OrderStatus.Cancelled) / totalOrders) * 100 : 0,
            ShippingCostRatio = grossRevenue > 0 ? (totalShippingFees / grossRevenue) * 100 : 0
        };
    }
}