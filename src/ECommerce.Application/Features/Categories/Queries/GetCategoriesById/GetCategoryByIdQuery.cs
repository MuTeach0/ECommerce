using ECommerce.Application.Features.Categories.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Categories.Queries.GetCategoriesById;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IRequest<Result<CategoryDTO>>;