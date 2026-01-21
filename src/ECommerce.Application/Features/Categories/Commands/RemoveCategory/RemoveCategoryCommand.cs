using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.RemoveCategory;

public record RemoveCategoryCommand(Guid Id) : IRequest<Result<Deleted>>;