using ECommerce.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // ده السطر اللي هيحل المشكلة
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title).HasMaxLength(100).IsRequired();
        builder.Property(a => a.City).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Street).HasMaxLength(150).IsRequired();
        builder.Property(a => a.FullAddress).HasMaxLength(500).IsRequired();

        builder.HasOne<Customer>()
               .WithMany(c => c.Addresses)
               .HasForeignKey(a => a.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}