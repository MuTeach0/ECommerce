using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Categories;
public static class CategoryErrors
{
    public static Error NameRequired =>
        Error.Validation("Category.NameRequired", "Category name is required.");

    public static Error DescriptionTooLong =>
        Error.Validation("Category.DescriptionTooLong", "Description cannot exceed 500 characters.");

    public static Error NotFound =>
        Error.NotFound("Category.NotFound", "The specified category was not found.");
}