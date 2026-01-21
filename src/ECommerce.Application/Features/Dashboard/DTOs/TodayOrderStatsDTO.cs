namespace ECommerce.Application.Features.Dashboard.DTOs;
public sealed class TodayOrderStatsDTO
{
    public DateOnly Date { get; init; }
    
    // Order Status Counters
    public int TotalOrders { get; init; }
    public int Pending { get; init; }
    public int Processing { get; init; }
    public int Shipped { get; init; }
    public int Delivered { get; init; }
    public int Cancelled { get; init; }
    public int Returned { get; init; }

    // Financial Metrics
    public decimal GrossRevenue { get; init; }      // إجمالي الإيرادات
    public decimal TotalProductCost { get; init; }  // تكلفة المنتجات على المتجر
    public decimal TotalShippingFees { get; init; } // إجمالي رسوم الشحن
    public decimal NetProfit { get; init; }         // صافي الربح
    public decimal ProfitMargin { get; init; }      // هامش الربح %

    // Customer & Product Metrics
    public int UniqueCustomers { get; init; }       // عدد العملاء الفريدين اليوم
    public int TotalItemsSold { get; init; }        // إجمالي عدد القطع المباعة

    // Performance Ratios (KPIs)
    public decimal AverageOrderValue { get; init; } // متوسط قيمة الطلب (AOV)
    public decimal ConversionRate { get; init; }    // نسبة التحويل (إذا كان لديك عدد الزيارات)
    public decimal ReturnRate { get; init; }        // نسبة المرتجعات
    public decimal CancellationRate { get; init; }  // نسبة الإلغاء
    public decimal ShippingCostRatio { get; init; } // نسبة تكلفة الشحن من الإيراد
}