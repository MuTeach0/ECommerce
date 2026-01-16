using ECommerce.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductItems.Commands.CreateProduct;

public class CreateProductItemCommandValidator : AbstractValidator<CreateProductItemCommand>
{
    private readonly IAppDbContext _context;
    public CreateProductItemCommandValidator(IAppDbContext context)
    {
        _context = context;
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Price).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(x => x.CostPrice)
            .WithMessage("Selling price must be greater than cost price.");
        RuleFor(v => v.CostPrice).GreaterThan(0);
        RuleFor(v => v.StockQuantity).GreaterThanOrEqualTo(0);

        RuleFor(v => v.SKU).NotEmpty().MaximumLength(50)
            .MustAsync(BeUniqueSKU).WithMessage("This SKU is already in use by another product.");
        RuleFor(v => v.CategoryId).NotEmpty()
            .MustAsync(CategoryExists).WithMessage("The specified Category does not exist.");
        RuleFor(v => v.CustomerId).NotEmpty()
            .MustAsync(CustomerExists).WithMessage("The specified Seller does not exist.");
    }

    private async Task<bool> BeUniqueSKU(string sku, CancellationToken ct) =>
        !await _context.ProductItems.AnyAsync(p => p.SKU == sku, ct);

    private async Task<bool> CategoryExists(Guid id, CancellationToken ct) =>
        await _context.Categories.AnyAsync(c => c.Id == id, ct);

    private async Task<bool> CustomerExists(Guid id, CancellationToken ct) =>
        await _context.Customers.AnyAsync(c => c.Id == id, ct);
}