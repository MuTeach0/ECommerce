using System.Text.Json;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Baskets;
using ECommerce.Domain.Common.Results;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerce.Infrastructure.Services;

public class BasketService(IDistributedCache cache) : IBasketService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
    };

    public async Task<Result<CustomerBasket>> GetBasketAsync(string customerId)
    {
        var data = await cache.GetStringAsync(customerId);
        if (string.IsNullOrEmpty(data))
        {
            return CustomerBasket.Create(customerId);
        }

        var basket = JsonSerializer.Deserialize<CustomerBasket>(data, _options);
        return basket ?? CustomerBasket.Create(customerId);
    }

    public async Task<Result<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket)
    {
        var data = JsonSerializer.Serialize(basket, _options);
        if (string.IsNullOrEmpty(data) || data == "{}")
            throw new Exception("Serialization failed! Basket is empty.");
        await cache.SetStringAsync(basket.Id, data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
        });
        return basket;
    }

    public async Task<Result<Deleted>> DeleteBasketAsync(string customerId)
    {
        await cache.RemoveAsync(customerId);
        return Result.Deleted;
    }
}