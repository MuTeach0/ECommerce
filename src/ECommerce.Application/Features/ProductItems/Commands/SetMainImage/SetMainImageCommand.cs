using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.ProductItems.Commands.SetMainImage;

public record SetMainImageCommand(Guid ProductId, Guid ImageId) : IRequest<Result<Updated>>;