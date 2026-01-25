using ECommerce.Domain.Common;

namespace  ECommerce.Domain.Customers.Items;

public sealed class ProductImage : AuditableEntity
{
    public string ImageUrl { get; private set; }
    public string PublicId { get; private set; } // ضروري للمسح من Cloudinary
    public bool IsMain { get; private set; }
    public Guid ProductId { get; private set; }

#pragma warning disable CS8618 
    private ProductImage() // عشان EF Core
    {
        // الـ compiler هيسكت لما نستخدم null! هنا
        ImageUrl = null!;
        PublicId = null!;
    }
#pragma warning restore CS8618
    internal ProductImage(Guid id, Guid productId, string imageUrl, string publicId, bool isMain)
        : base(id)
    {
        ProductId = productId;
        ImageUrl = imageUrl;
        PublicId = publicId;
        IsMain = isMain;
    }

    public void SetAsMain() => IsMain = true;
    public void UnsetMain() => IsMain = false;
}