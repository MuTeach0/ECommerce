using ECommerce.Domain.Baskets;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Common.Interfaces;

public interface IBasketService
{
    Task<Result<CustomerBasket>> GetBasketAsync(string customerId);
    Task<Result<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket);
    Task<Result<Deleted>> DeleteBasketAsync(string customerId);
}