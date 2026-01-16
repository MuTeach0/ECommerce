using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.ProductItems.DTOs;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.ProductItems.Queries.GetProducts;

public class GetProductsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetProductsQuery, Result<List<ProductItemDTO>>>
{
    public async Task<Result<List<ProductItemDTO>>> Handle(GetProductsQuery request, CancellationToken ct)
    {
  //  ProductItem
        var products = await context.ProductItems
             .AsNoTracking()
             .Include(p => p.Category)
             .Include(p => p.Customer)
             .Where(p => p.IsActive)
             .Select(p => new ProductItemDTO(
                 p.Id,
                 p.Name,
                 p.Description,
                 p.Price,
                 p.DiscountPrice,
                 p.StockQuantity,
                 p.SKU,
                 p.CategoryId,
                 p.Category != null ? p.Category.Name : null,
                 p.CustomerId,
                 p.Customer != null ? p.Customer.Name : null,
                 p.AverageRating,
                 p.ReviewsCount)).ToListAsync(ct);

        return products;
    }
}