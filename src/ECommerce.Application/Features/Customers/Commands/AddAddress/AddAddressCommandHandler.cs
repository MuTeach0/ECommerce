using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Customers.Commands.AddAddress;

public class AddAddressCommandHandler(
    IAppDbContext context,
    ILogger<AddAddressCommandHandler> logger,
    IUser user)
    : IRequestHandler<AddAddressCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddAddressCommand request, CancellationToken ct)
    {
        // Get the current user ID from the Identity Service
        var userIdString = user.Id;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var customerId))
        {
            logger.LogWarning("Unauthorized access attempt or invalid User ID in Token.");
            return Error.Unauthorized();
        }

        logger.LogInformation("Processing AddAddressCommand for CustomerId: {CustomerId}", customerId);

        try
        {
            // 1. Fetch the customer with their existing addresses
            // Include is necessary to load the addresses collection into memory
            var customer = await context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == customerId, ct);

            if (customer is null)
            {
                logger.LogWarning("Customer not found. CustomerId: {CustomerId}", customerId);
                return Error.NotFound("Customer.NotFound", "Customer does not exist.");
            }

            // 2. Use the Domain Method
            // This method creates a new Address entity and adds it to the customer's collection
            var result = customer.AddAddress(
                request.Title,
                request.City,
                request.Street,
                request.FullAddress);

            if (result.IsError)
                return result.Errors;

            // 3. Save Changes
            // EF Core tracks the new entry in the collection and performs the INSERT automatically
            await context.SaveChangesAsync(ct);

            // Retrieve the ID of the newly added address
            var newAddressId = customer.Addresses.Last().Id;

            logger.LogInformation("Address successfully created. AddressId: {AddressId}", newAddressId);

            return newAddressId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding address for Customer: {CustomerId}", customerId);
            return Error.Failure("Address.AddFailed", "An unexpected error occurred.");
        }
    }
}