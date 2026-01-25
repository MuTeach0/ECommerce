using ECommerce.Domain.Customers.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
       // 1. Primary Key configuration (Using non-clustered index is optimized for GUIDs)
       builder.HasKey(i => i.Id).IsClustered(false);
       builder.Property(i => i.Id).ValueGeneratedNever();

       // 2. Basic Information properties
       builder.Property(i => i.Name)
              .IsRequired()
              .HasMaxLength(200);

       builder.Property(i => i.Description)
              .HasMaxLength(2000);

       // 3. Financial properties (Ensuring high precision for decimal values)
       builder.Property(i => i.Price)
              .HasColumnType("decimal(18,2)")
              .IsRequired();

       builder.Property(i => i.DiscountPrice)
              .HasColumnType("decimal(18,2)");

       builder.Property(i => i.CostPrice)
              .HasColumnType("decimal(18,2)")
              .IsRequired();

       // 4. Inventory and Identification logic
       builder.Property(i => i.SKU)
              .IsRequired()
              .HasMaxLength(50);

       // SKU must be unique across the entire system
       builder.HasIndex(i => i.SKU).IsUnique();

       builder.Property(i => i.StockQuantity)
              .IsRequired();

       builder.Property(i => i.IsActive)
              .HasDefaultValue(true);

       // 5. Ratings and Feedback metrics
       builder.Property(i => i.AverageRating)
              .HasPrecision(3, 1) // Allows values like 4.5 (total 3 digits, 1 after decimal)
              .HasDefaultValue(0.0);

       builder.Property(i => i.ReviewsCount)
              .HasDefaultValue(0);

       // 6. Relationships

       // Category Relationship: One Category can have many ProductItems
       builder.HasOne(i => i.Category)
              .WithMany(c => c.Products)
              .HasForeignKey(i => i.CategoryId)
              .OnDelete(DeleteBehavior.Restrict); // Prevent category deletion if it contains products

       // Images Relationship: One ProductItem can have many ProductImages
       builder.HasMany(i => i.Images)
              .WithOne() // No navigation property back to ProductItem in the Image entity
              .HasForeignKey(img => img.ProductId)
              .OnDelete(DeleteBehavior.Cascade); // Automatically delete images when product is removed

       // 7. Metadata and Access Configuration
       // Enforce EF Core to use the backing field for the Images collection to respect encapsulation
       builder.Navigation(i => i.Images)
              .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}