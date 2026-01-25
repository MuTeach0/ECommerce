using ECommerce.Application.Features.Payments.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Payments.Queries.GetOrderPayment;

public sealed record GetOrderPaymentQuery(Guid OrderId) : IRequest<Result<PaymentDTO>>;