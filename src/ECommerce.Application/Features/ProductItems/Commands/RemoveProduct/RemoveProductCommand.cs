using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.ProductItems.Commands.RemoveProduct;


public sealed record RemoveProductCommand(Guid ProductId) : IRequest<Result<Deleted>>;