using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers;
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
        // 1. استخراج الـ User ID من الـ Token
        var userIdString = user.Id;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var customerId))
        {
            logger.LogWarning("Unauthorized access attempt or invalid User ID.");
            return Error.Unauthorized();
        }

        try
        {
            // 2. التحقق من وجود العميل باستخدام AnyAsync (بدون تحميل الكائن وبدون Tracking)
            // هذه الخطوة سريعة جداً ولا تسبب Concurrency Conflicts
            var customerExists = await context.Customers
                .AnyAsync(c => c.Id == customerId, ct);

            if (!customerExists)
            {
                logger.LogWarning("Customer not found: {CustomerId}", customerId);
                return Error.NotFound("Customer.NotFound", "Customer does not exist.");
            }

            // 3. إنشاء كائن العنوان باستخدام الـ Domain Logic (Factory Method)
            // نمرر الـ customerId يدوياً هنا للربط (Foreign Key)
            var addressResult = Address.Create(
                customerId,
                request.Title,
                request.City,
                request.Street,
                request.FullAddress);

            if (addressResult.IsError)
                return addressResult.Errors;

            var newAddress = addressResult.Value;

            // 4. الإضافة المباشرة لجدول العناوين
            // هنا الـ EF والـ Interceptor سيلاحظان "إضافة" (Add) فقط لعنوان جديد
            // ولن يتم المساس بجدول الـ Customers إطلاقاً
            context.Addresses.Add(newAddress);

            // 5. حفظ التغييرات
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Address successfully created for Customer: {CustomerId}. AddressId: {AddressId}",
                customerId, newAddress.Id);

            return newAddress.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during AddAddress for Customer: {CustomerId}", customerId);
            return Error.Failure("Address.AddFailed", "An unexpected error occurred while saving the address.");
        }
    }

    // public async Task<Result<Guid>> Handle(AddAddressCommand request, CancellationToken ct)
    // {
    //     // Get the current user ID from the Identity Service
    //     var userIdString = user.Id;

    //     if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var customerId))
    //     {
    //         logger.LogWarning("Unauthorized access attempt or invalid User ID in Token.");
    //         return Error.Unauthorized();
    //     }

    //     logger.LogInformation("Processing AddAddressCommand for CustomerId: {CustomerId}", customerId);

    //     try
    //     {
    //         // 1. Fetch the customer with their existing addresses
    //         // Include is necessary to load the addresses collection into memory
    //         var customer = await context.Customers

    //             .Include(c => c.Addresses)
    //             .FirstOrDefaultAsync(c => c.Id == customerId, ct);

    //         if (customer is null)
    //         {
    //             logger.LogWarning("Customer not found. CustomerId: {CustomerId}", customerId);
    //             return Error.NotFound("Customer.NotFound", "Customer does not exist.");
    //         }

    //         // 2. Use the Domain Method
    //         // This method creates a new Address entity and adds it to the customer's collection
    //         var result = customer.AddAddress(
    //             request.Title,
    //             request.City,
    //             request.Street,
    //             request.FullAddress);

    //         if (result.IsError)
    //             return result.Errors;
    //         // 3. Save Changes
    //         // EF Core tracks the new entry in the collection and performs the INSERT automatically
    //         await context.SaveChangesAsync(ct);

    //         // Retrieve the ID of the newly added address
    //         var newAddressId = customer.Addresses.Last().Id;

    //         logger.LogInformation("Address successfully created. AddressId: {AddressId}", newAddressId);

    //         return newAddressId;
    //     }
    //     catch (DbUpdateConcurrencyException ex)
    //     {
    //         // هذا الخطأ تحديداً يعني أن الـ EF يحاول تحديث بيانات قديمة
    //         logger.LogError(ex, "Concurrency error occurred for Customer: {CustomerId}", customerId);
    //         return Error.Failure("Address.ConcurrencyError", "The data has been modified by another process. Please try again.");
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogError(ex, "Error adding address for Customer: {CustomerId}", customerId);
    //         return Error.Failure("Address.AddFailed", "An unexpected error occurred.");
    //     }
    // }
}