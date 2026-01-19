using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Customers.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Customers.Queries;

public class GetUserAddressesQueryHandler(IAppDbContext context, IUser userService)
    : IRequestHandler<GetUserAddressesQuery, Result<IReadOnlyList<AddressDTO>>>
{
    public async Task<Result<IReadOnlyList<AddressDTO>>> Handle(GetUserAddressesQuery request, CancellationToken ct)
    {
        // 1. Retrieve the current User ID from the Identity Service
        var userIdString = userService.Id;

        // 2. Validate the identity
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var customerId))
        {
            return Error.Unauthorized();
        }

        // 3. Fetch addresses and project them directly to DTOs
        // Using AsNoTracking for improved read performance
        var addresses = await context.Addresses
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .Select(a => new AddressDTO(
                a.Id,
                a.Title,
                a.City,
                a.Street,
                a.FullAddress))
            .ToListAsync(ct);

        return addresses.AsReadOnly();
    }
}