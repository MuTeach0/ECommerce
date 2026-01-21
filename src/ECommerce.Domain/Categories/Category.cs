using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;

namespace ECommerce.Domain.Categories;
public sealed class Category : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    // لتنظيم الفئات بشكل هرمي (مثلاً فئة فرعية تنتمي لفئة رئيسية)
    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; } // Navigation Property
    // الربط مع المنتجات
    private readonly List<ProductItem> _products = new();
    public IReadOnlyCollection<ProductItem> Products => _products.AsReadOnly();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Category() { } // For EF Core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Category(Guid id, string name, string description, string? imageUrl, Guid? parentCategoryId)
        : base(id)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        ParentCategoryId = parentCategoryId;
        IsActive = true;
    }
    public void AddProduct(ProductItem product) => _products.Add(product);
    public static Result<Category> Create(Guid id, string name, string description, string? imageUrl, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CategoryErrors.NameRequired;

        if (description.Length > 500)
            return CategoryErrors.DescriptionTooLong;

        return new Category(id, name, description, imageUrl, parentCategoryId);
    }

    public Result<Updated> Update(string? name, string? description, string? imageUrl, Guid? parentCategoryId)
    {
        if (name != null && string.IsNullOrWhiteSpace(name))
            return CategoryErrors.NameRequired;

        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        
        if (string.IsNullOrWhiteSpace(description))
            Description = Description;

        if (!string.IsNullOrWhiteSpace(imageUrl))
            ImageUrl = imageUrl;

        if (parentCategoryId.HasValue && parentCategoryId != Guid.Empty)
        {
            // حماية بسيطة: القسم ميكونش أب لنفسه
            if (parentCategoryId != Id)
            {
                ParentCategoryId = parentCategoryId;
            }
        }

        //Name = name;
        //Description = description;
        //ImageUrl = imageUrl;
        //ParentCategoryId = parentCategoryId;

        return Result.Updated;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}