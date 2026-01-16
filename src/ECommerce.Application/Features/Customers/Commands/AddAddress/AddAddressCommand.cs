using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Customers.Commands.AddAddress;

public record AddAddressCommand(
    Guid CustomerId,
    string Title,
    string City,
    string Street,
    string FullAddress) : IRequest<Result<Guid>>;