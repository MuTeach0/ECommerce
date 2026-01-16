using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Reviews.Commands.AddReview;

public sealed record AddReviewCommand(
    Guid ProductId,
    int Stars,
    string Comment) : IRequest<Result<Guid>>;