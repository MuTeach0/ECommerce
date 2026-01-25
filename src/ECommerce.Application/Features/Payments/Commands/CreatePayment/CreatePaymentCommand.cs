using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand(Guid OrderId) : IRequest<Result<string>>;