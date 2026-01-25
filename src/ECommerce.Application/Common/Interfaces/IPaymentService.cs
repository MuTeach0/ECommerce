using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<string>> CreateOrderAsync(decimal amount, string currency);
    
    Task<Result<bool>> CaptureOrderAsync(string transactionId);
}