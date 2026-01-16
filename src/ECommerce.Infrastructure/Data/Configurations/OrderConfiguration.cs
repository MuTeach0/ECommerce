using ECommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // 1. المفتاح الأساسي
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        // 2. الربط بجدول العناوين (التعديل الجديد)
        builder.Property(o => o.AddressId)
            .IsRequired();

        builder.HasOne(o => o.Address)
            .WithMany() // العنوان الواحد قد يرتبط بأكثر من أوردر (تاريخياً)
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.Restrict); // منع حذف العنوان إذا كان مرتبطاً بأوردر نشط

        // 3. إعدادات الحقول الأساسية
        builder.Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)");

        // 4. حالة الطلب
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // 5. العلاقة مع العميل (المشتري)
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 6. العلاقة مع عناصر الطلب (OrderItems)
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // 7. تجاهل خاصية TotalAmount (محسوبة في الكود)
        builder.Ignore(o => o.TotalAmount);
    }
}