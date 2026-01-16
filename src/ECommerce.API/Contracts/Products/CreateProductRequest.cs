namespace ECommerce.API.Contracts.Products;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    decimal CostPrice,
    int StockQuantity,
    string SKU,
    Guid CategoryId);