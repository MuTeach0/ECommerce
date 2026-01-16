using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    HybridCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
{
    private readonly HybridCache _cache = cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;


    //public async Task<TResponse> Handle(
    //    TRequest request,
    //    RequestHandlerDelegate<TResponse> next,
    //    CancellationToken cancellationToken)
    //{
    //    logger.LogInformation("Handling cache for {RequestName}", typeof(TRequest).Name);

    //    return await cache.GetOrCreateAsync(
    //        request.CacheKey,
    //        async factoryToken => await next(factoryToken),
    //        new HybridCacheEntryOptions { Expiration = request.Expiration },
    //        tags: request.Tags,
    //        cancellationToken: cancellationToken);
    //}
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery cachedRequest)
        {
            return await next(cancellationToken);
        }

        _logger.LogInformation("Checking cache for request {RequestName}", typeof(TRequest).Name);

        var result = await _cache.GetOrCreateAsync<TResponse>(
            cachedRequest.CacheKey,
            _ => new ValueTask<TResponse>((TResponse)(object)null!),
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableUnderlyingData
            },
            cancellationToken: cancellationToken);

        if (result is null)
        {
            result = await next(cancellationToken);

            if (result is IResult res && res.IsSuccess)
            {
                _logger.LogInformation("Caching result for {RequestName}", typeof(TRequest).Name);
                await _cache.SetAsync(
                    cachedRequest.CacheKey,
                    result,
                    new HybridCacheEntryOptions
                    {
                        Expiration = cachedRequest.Expiration
                    },
                    cachedRequest.Tags,
                    cancellationToken);
            }
        }


        return result;
    }
}