using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    Guid? ParentCategoryId) : IRequest<Result<Updated>>;