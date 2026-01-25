using ECommerce.Application.Features.Payments.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Payments.Queries.GetPayment;

public record GetPaymentQuery(Guid PaymentId) : IRequest<Result<PaymentDTO>>;