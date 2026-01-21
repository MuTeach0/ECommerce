using ECommerce.Application.Features.Reviews.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Reviews.Queries.GetProductReviews;

public sealed record GetProductReviewsQuery(Guid ProductId) : IRequest<Result<IReadOnlyList<ProductReviewDTO>>>;