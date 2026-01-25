using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.CapturePayment;

public sealed record CapturePaymentCommand(string PayPalOrderId) : IRequest<Result<bool>>;