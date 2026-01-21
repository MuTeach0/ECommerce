namespace ECommerce.Application.Features.Categories.DTOs;

public record CategoryDTO(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    string? ParentCategoryName
);