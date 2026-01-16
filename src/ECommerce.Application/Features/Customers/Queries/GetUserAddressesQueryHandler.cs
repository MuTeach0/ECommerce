using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Customers.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Customers.Queries;

public class GetUserAddressesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetUserAddressesQuery, Result<IReadOnlyList<AddressDTO>>>
{
    public async Task<Result<IReadOnlyList<AddressDTO>>> Handle(GetUserAddressesQuery request, CancellationToken ct)
    {
        // جلب العناوين وتحويلها مباشرة لـ DTO
        var addresses = await context.Addresses
            .AsNoTracking()
            .Where(a => a.CustomerId == request.CustomerId)
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