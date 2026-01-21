using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.ProductItems.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    decimal CostPrice,
    int StockQuantity,
    string SKU,
    Guid CategoryId) : IRequest<Result<Updated>>;