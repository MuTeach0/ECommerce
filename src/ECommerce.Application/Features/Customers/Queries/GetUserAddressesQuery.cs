using ECommerce.Application.Features.Customers.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Customers.Queries;

public sealed record GetUserAddressesQuery() : IRequest<Result<IReadOnlyList<AddressDTO>>>;