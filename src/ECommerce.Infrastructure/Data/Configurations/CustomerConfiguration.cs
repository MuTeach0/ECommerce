using ECommerce.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id).IsClustered(false);

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

        builder.Property(c => c.Email)
               .HasMaxLength(150);
        builder.HasIndex(c => c.Email).IsUnique();       
        builder.HasMany(c => c.Orders)
              .WithOne(o => o.Customer) // حدد أن الطلب يخص هذا العميل
              .HasForeignKey(o => o.CustomerId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(c => c.Orders)
              .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.OwnedProducts)
              .WithOne(p => p.Customer)
              .HasForeignKey(p => p.CustomerId);


        builder.Navigation(c => c.OwnedProducts)
              .UsePropertyAccessMode(PropertyAccessMode.Field);


        builder.HasMany(c => c.Addresses)
           .WithOne()
           .HasForeignKey(a => a.CustomerId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}