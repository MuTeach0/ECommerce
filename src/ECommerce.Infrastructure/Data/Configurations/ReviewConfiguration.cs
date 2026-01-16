using ECommerce.Domain.Customers.Items;
using ECommerce.Domain.Customers.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired();

        // ربط التقييم بالمنتج (One-to-Many)
        builder.HasOne<ProductItem>()
            .WithMany() // لو مش عامل Collection للـ Reviews جوه الـ ProductItem
            .HasForeignKey(r => r.ProductItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // ربط التقييم بالعميل (حسب جدول الـ Users عندك)
        builder.Property(r => r.CustomerId).IsRequired();
    }
}