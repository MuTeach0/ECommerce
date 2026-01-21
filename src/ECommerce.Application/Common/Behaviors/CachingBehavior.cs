using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Common.Behaviors;
public class CachingBehavior<TRequest, TResponse>(
    HybridCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger,
    IUser userService) // Injected IUser to resolve identity for cache keys
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
{
    private readonly HybridCache _cache = cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IUser _userService = userService;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 1. Check if the request implements ICachedQuery
        if (request is not ICachedQuery cachedRequest)
        {
            return await next(cancellationToken);
        }

        // 2. Resolve Cache Key including User Identity if applicable
        // Note: You might want to update your ICachedQuery interface to accept a userId
        // or ensure the CacheKey property in the Query record handles identity internally.
        string cacheKey = cachedRequest.CacheKey;
        
        // If the request is user-specific, we append the userId to the key
        // This ensures User A doesn't get User B's cached data.
        if (!string.IsNullOrEmpty(_userService.Id))
        {
            cacheKey = $"{cacheKey}-{_userService.Id}";
        }

        _logger.LogInformation("Checking cache for request {RequestName} with Key: {CacheKey}", 
            typeof(TRequest).Name, cacheKey);

        // 3. Try to get data from cache
        var result = await _cache.GetOrCreateAsync<TResponse>(
            cacheKey,
            _ => new ValueTask<TResponse>((TResponse)(object)null!),
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableUnderlyingData
            },
            cancellationToken: cancellationToken);

        if (result is not null)
        {
            _logger.LogInformation("Cache hit for {RequestName}", typeof(TRequest).Name);
            return result;
        }

        // 4. Cache miss: Execute the handler
        result = await next(cancellationToken);

        // 5. Only cache successful results (avoid caching error messages)
        if (result is IResult res && res.IsSuccess)
        {
            _logger.LogInformation("Caching result for {RequestName} with expiration {Expiration}", 
                typeof(TRequest).Name, cachedRequest.Expiration);

            await _cache.SetAsync(
                cacheKey,
                result,
                new HybridCacheEntryOptions
                {
                    Expiration = cachedRequest.Expiration
                },
                cachedRequest.Tags,
                cancellationToken);
        }

        return result;
    }
}