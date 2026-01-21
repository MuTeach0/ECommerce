using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Identity.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string PhoneNumber) : IRequest<Result<string>>;