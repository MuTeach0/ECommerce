namespace ECommerce.Domain.Common.Constants;

public static class ECommerceConstants
{
    public const string DefaultCurrency = "USD";
    public const int MaxProductNameLength = 100;
    public const int MaxDescriptionLength = 1000;
    public const int MaxCategoryNameLength = 50;
    public const decimal MinOrderAmount = 0.01m;
    public const int MaxItemsPerOrder = 100;
}