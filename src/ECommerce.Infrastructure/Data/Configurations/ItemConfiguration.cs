using ECommerce.Domain.Customers.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
   public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        // 1. Primary Key configuration (Non-clustered is good for GUIDs)
        builder.HasKey(i => i.Id).IsClustered(false);
        builder.Property(i => i.Id).ValueGeneratedNever();

        // 2. Basic Info configuration
        builder.Property(i => i.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(i => i.Description)
               .HasMaxLength(2000); // Descriptions can be long

        // 3. Financial configuration (Crucial for precision)
        builder.Property(i => i.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(i => i.DiscountPrice)
               .HasColumnType("decimal(18,2)");

        // 4. Inventory & Logic
        builder.Property(i => i.SKU)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(i => i.SKU).IsUnique(); // SKU must be unique in an e-commerce system

        builder.Property(i => i.StockQuantity)
               .IsRequired();

        builder.Property(i => i.IsActive)
               .HasDefaultValue(true);

        // 5. Relationships

        // Relation with Category (One Category has many Items)
        builder.HasOne(i => i.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(i => i.CategoryId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting category if it has items

        // Relation with Customer (One Customer/Seller has many Items)
        builder.HasOne(i => i.Customer)
               .WithMany(c => c.OwnedProducts) // Ensure you have "Items" collection in Customer class
               .HasForeignKey(i => i.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

       // 6. Cost Price configuration (Internal use only)
       builder.Property(i => i.CostPrice)
       .HasColumnType("decimal(18,2)")
       .IsRequired();

        // 7. Ratings configuration (New Fields)
        builder.Property(i => i.AverageRating)
               .HasPrecision(3, 1) // يسمح بـ 3 أرقام إجمالاً ورقم واحد بعد العلامة (مثل 4.5)
               .HasDefaultValue(0.0);

        builder.Property(i => i.ReviewsCount)
               .HasDefaultValue(0);
    }
}
