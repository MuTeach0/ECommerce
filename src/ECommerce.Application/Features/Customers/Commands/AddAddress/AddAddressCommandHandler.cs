using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Customers.Commands.AddAddress;

public class AddAddressCommandHandler(
    IAppDbContext context,
    ILogger<AddAddressCommandHandler> logger)
    : IRequestHandler<AddAddressCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddAddressCommand request, CancellationToken ct)
    {
        logger.LogInformation("Processing AddAddressCommand for CustomerId: {CustomerId}", request.CustomerId);

        try
        {
            // 1. جلب العميل مع عناوينه الحالية 
            // الـ Include مهم هنا عشان الـ Collection اللي جوه الـ Entity تتملي
            var customer = await context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct);

            if (customer is null)
            {
                logger.LogWarning("Customer not found. CustomerId: {CustomerId}", request.CustomerId);
                return Error.NotFound("Customer.NotFound", "Customer does not exist.");
            }

            // 2. استخدام ميثود الدومين (هي اللي بتعمل New Address وبتضيفه للـ List)
            var result = customer.AddAddress(request.Title, request.City, request.Street, request.FullAddress);

            if (result.IsError) return result.Errors;

            // 3. الحفظ مباشرة
            // الـ EF Core هيفهم لوحده إن فيه سجل جديد اتضاف في الـ Addresses Collection
            // وهيعمل INSERT للعنوان الجديد فقط بدون تعديل الـ Customer
            await context.SaveChangesAsync(ct);

            // جلب الـ ID بتاع العنوان اللي لسه مضاف
            var newAddressId = customer.Addresses.Last().Id;

            logger.LogInformation("Address successfully created. AddressId: {AddressId}", newAddressId);

            return newAddressId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding address for Customer: {CustomerId}", request.CustomerId);
            return Error.Failure("Address.AddFailed", "An unexpected error occurred.");
        }
    }
}