using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Contracts.Products;

public sealed record CreateProductRequest(
    [Required] string Name,
    string Description,
    [Range(0.01, 1000000)] decimal Price,
    [Range(0.01, 1000000)] decimal CostPrice,
    [Range(0, 10000)] int StockQuantity,
    [Required] string SKU,
    [Required] Guid CategoryId);