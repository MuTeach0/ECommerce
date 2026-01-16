using ECommerce.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
       public void Configure(EntityTypeBuilder<Category> builder)
       {
       // 1. Primary Key
       builder.HasKey(c => c.Id).IsClustered(false);
       builder.Property(c => c.Id).ValueGeneratedNever();

       // 2. Properties
       builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
       builder.Property(c => c.Description).HasMaxLength(500);
       builder.Property(c => c.ImageUrl).HasMaxLength(2048);
       builder.Property(c => c.IsActive).HasDefaultValue(true);

       // 3. العلاقة الهرمية الصحيحة (دمجنا التعريفين في واحد)
       builder.HasOne(c => c.ParentCategory) // نستخدم الـ Navigation Property
              .WithMany()                    // قسم واحد يمكن أن يكون أباً لأقسام كثيرة
              .HasForeignKey(c => c.ParentCategoryId) // المفتاح الأجنبي
              .OnDelete(DeleteBehavior.Restrict);     // منع حذف أب له أبناء

       // 4. العلاقة مع المنتجات
       builder.HasMany(c => c.Products)
              .WithOne(p => p.Category)
              .HasForeignKey(p => p.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);
       }
}