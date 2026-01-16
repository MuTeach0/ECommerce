using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(
    IAppDbContext context,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> // يفضل استهداف الـ Commands فقط لو عندك Interface لها
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 1. لو الطلب عبارة عن Query (قراءة بس)، نعديه من غير Transaction
        if (request.GetType().Name.EndsWith("Query"))
        {
            return await next(cancellationToken);
        }

        logger.LogInformation("Beginning transaction for {RequestName}", typeof(TRequest).Name);

        using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);

            // 2. لو الـ Handler رجع نجاح، نعمل Commit
            if (response.IsSuccess)
            {
                logger.LogInformation("Committing transaction for {RequestName}", typeof(TRequest).Name);
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                // 3. لو الـ Handler رجع Error (Result.IsError)، نعمل Rollback
                logger.LogWarning("Rolling back transaction for {RequestName} due to validation/business error", typeof(TRequest).Name);
                await transaction.RollbackAsync(cancellationToken);
            }

            return response;
        }
        catch (Exception ex)
        {
            // 4. لو حصل Exception غير متوقع، نضمن عمل Rollback قبل ما الـ UnhandledExceptionBehavior يمسكه
            logger.LogError(ex, "Transaction failed and rolled back for {RequestName}", typeof(TRequest).Name);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}