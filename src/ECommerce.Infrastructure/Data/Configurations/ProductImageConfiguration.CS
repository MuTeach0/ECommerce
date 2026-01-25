using ECommerce.Domain.Customers.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(img => img.Id);
        builder.Property(img => img.Id).ValueGeneratedNever();

        builder.Property(img => img.ImageUrl)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(img => img.PublicId)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(img => img.IsMain)
               .HasDefaultValue(false);
    }
}