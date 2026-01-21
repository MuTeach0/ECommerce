using ECommerce.Domain.Orders.OrderItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).ValueGeneratedNever();

        builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.CostPrice).HasColumnType("decimal(18,2)");

        builder.HasOne(oi => oi.ProductItem)
            .WithMany()
            .HasForeignKey(oi => oi.ProductItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}