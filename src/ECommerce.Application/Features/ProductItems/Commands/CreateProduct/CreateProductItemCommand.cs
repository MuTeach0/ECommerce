using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.ProductItems.Commands.CreateProduct;

public sealed record CreateProductItemCommand(
    string Name,
    string Description,
    decimal Price,
    decimal CostPrice,
    int StockQuantity,
    string SKU,
    Guid CategoryId,
    Guid CustomerId
) : IRequest<Result<Guid>>;