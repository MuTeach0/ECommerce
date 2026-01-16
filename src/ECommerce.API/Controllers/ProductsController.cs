using Asp.Versioning;
using ECommerce.API.Contracts.Products;
using ECommerce.Application.Features.ProductItems.Commands.CreateProduct;
using ECommerce.Application.Features.ProductItems.Commands.RemoveProduct;
using ECommerce.Application.Features.ProductItems.Commands.UpdateProduct;
using ECommerce.Application.Features.ProductItems.DTOs;
using ECommerce.Application.Features.ProductItems.Queries.GetProducts;
using ECommerce.Application.Features.ProductItems.Queries.GetProductsById;
using ECommerce.Application.Features.Reviews.Commands.AddReview;
using ECommerce.Application.Features.Reviews.DTOs;
using ECommerce.Application.Features.Reviews.Queries.GetProductReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;
using ECommerce.Domain.Common.Results;

namespace ECommerce.API.Controllers;

[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[Authorize]
public sealed class ProductsController(ISender sender, IOutputCacheStore cacheStore) : ApiController
{
    #region Product Management (CRUD)

    [HttpPost]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Seller adds a new product.")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var customerGuid))
            return Unauthorized();

        var command = new CreateProductItemCommand(
            request.Name, request.Description, request.Price, request.CostPrice,
            request.StockQuantity, request.SKU, request.CategoryId, customerGuid
        );

        var result = await sender.Send(command, ct);

        // Evict generic product list cache
        await cacheStore.EvictByTagAsync("products_list", ct);

        return result.Match(
            id => CreatedAtAction(nameof(GetById), new { version = "1.0", productId = id }, id),
            Problem);
    }

    [HttpGet]
    [AllowAnonymous] // Usually products are public
    [OutputCache(Duration = 60)]
    [ProducesResponseType(typeof(List<ProductItemDTO>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves all products.")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetProductsQuery(), ct);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{productId:guid}", Name = "GetProductById")]
    [AllowAnonymous]
    [OutputCache(Duration = 60, VaryByRouteValueNames = ["productId"])]
    [ProducesResponseType(typeof(ProductItemDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a product by ID.")]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductByIdQuery(productId), ct);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPut("{productId:guid}")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates an existing product.")]
    public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            productId,
            request.Name,
            request.Description,
            request.Price,
            request.CostPrice,
            request.StockQuantity,
            request.SKU,
            request.CategoryId);

        var result = await sender.Send(command, ct);
        await cacheStore.EvictByTagAsync($"product_{productId}", ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("{productId:guid}")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes a product.")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveProductCommand(productId), ct);
        await cacheStore.EvictByTagAsync("products_list", ct);
        return result.Match(_ => NoContent(), Problem);
    }

    #endregion

    #region Product Reviews

    [HttpGet("{productId:guid}/reviews")]
    [AllowAnonymous]
    [OutputCache(Duration = 120, VaryByRouteValueNames = ["productId"])]
    [ProducesResponseType(typeof(IReadOnlyList<ProductReviewDTO>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves reviews for a specific product.")]
    public async Task<IActionResult> GetProductReviews(Guid productId, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductReviewsQuery(productId), ct);
        return result.Match(reviews => Ok(reviews), Problem);
    }

    [HttpPost("{productId:guid}/reviews")]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Adds a review to a product.")]
    public async Task<IActionResult> AddReview(Guid productId, [FromBody] AddReviewRequest request, CancellationToken ct)
    {
        var command = new AddReviewCommand(productId, request.Stars, request.Comment);
        var result = await sender.Send(command);

        //return result.Match(
        //    id => CreatedAtAction(
        //        nameof(GetProductReviews),
        //        new { version = "1.0", productId = productId },
        //        id),
        //    Problem);
        // Using the new MatchAsync extension
        // Corrected MatchAsync implementation with proper closing braces
        // We explicitly specify <Guid, IActionResult> to solve the conversion error
        return await result.MatchAsync<Guid, IActionResult>(
            async id =>
            {
                // Clear cache to show updated data immediately
                await cacheStore.EvictByTagAsync("products_list", ct);

                // Explicitly cast to IActionResult
                return (IActionResult)CreatedAtAction(
                    nameof(GetProductReviews),
                    new { version = "1.0", productId = productId },
                    id);
            },
            errors => {
                // Explicitly return as Task<IActionResult>
                return Task.FromResult<IActionResult>(Problem(errors));
            }
        );
    }

    #endregion

    public record AddReviewRequest(int Stars, string Comment);
}