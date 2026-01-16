namespace ECommerce.API.Contracts.Categories;

public sealed record UpdateCategoryRequest(
    string Name,
    string Description,
    string? ImageUrl,
    Guid? ParentCategoryId);