using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Contracts.Products;

public sealed record UpdateProductRequest(
    [Required] string Name,
    string Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0.01, double.MaxValue)] decimal CostPrice,
    [Range(0, int.MaxValue)] int StockQuantity,
    [Required] string SKU,
    [Required] Guid CategoryId
);