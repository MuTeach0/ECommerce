using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Description,
    string? ImageUrl,
    Guid? ParentCategoryId) : IRequest<Result<Guid>>;