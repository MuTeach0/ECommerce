using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.Features.ProductItems.Commands.AddProductImage;

public sealed record AddProductImageCommand(
    Guid ProductId,
    List<IFormFile> Files,
    bool IsMain = false
) : IRequest<Result<Guid>>;