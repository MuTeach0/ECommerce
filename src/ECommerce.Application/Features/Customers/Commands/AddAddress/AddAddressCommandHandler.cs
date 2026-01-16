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
            // 1. التأكد من وجود العميل (بدون الحاجة لعمل Include للعناوين كلها لتوفير الأداء)
            var customerExists = await context.Customers.AnyAsync(c => c.Id == request.CustomerId, ct);

            if (!customerExists)
            {
                logger.LogWarning("Customer not found. CustomerId: {CustomerId}", request.CustomerId);
                return Error.NotFound("Customer.NotFound", "Customer does not exist.");
            }

            // 2. إنشاء كائن العنوان الجديد (تأكد أن الـ Domain Model يسمح بإنشاء Address مستقلاً أو عبر ميثود ثابتة)
            // هنا سنستخدم الميثود اللي في الـ Customer لضمان تطبيق الـ Logic الخاص بك
            var customer = await context.Customers.FirstAsync(c => c.Id == request.CustomerId, ct);
            var result = customer.AddAddress(request.Title, request.City, request.Street, request.FullAddress);

            if (result.IsError) return result.Errors;

            // 3. التعديل الجوهري: إخبار الـ EF صراحة أن هناك عنوان جديد تم إضافته
            var newAddress = customer.Addresses.Last();

            // نضمن أن الـ EF سيقوم بعمل Insert وليس Update
            context.Addresses.Add(newAddress);

            // 4. الحفظ
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Address successfully created. AddressId: {AddressId}", newAddress.Id);

            return newAddress.Id;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Concurrency error: The customer record might have been modified.");
            return Error.Failure("Address.ConcurrencyError", "Database synchronization failed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding address for Customer: {CustomerId}", request.CustomerId);
            return Error.Failure("Address.AddFailed", ex.Message);
        }
    }
}