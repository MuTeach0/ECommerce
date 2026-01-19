using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Customers.Commands.AddAddress;

public record AddAddressCommand(
    string Title,
    string City,
    string Street,
    string FullAddress) : IRequest<Result<Guid>>;