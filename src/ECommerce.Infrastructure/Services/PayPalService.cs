using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
namespace ECommerce.Infrastructure.Services;
public class PayPalService(HttpClient httpClient, IOptions<PayPalSettings> settings) : IPaymentService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly PayPalSettings _settings = settings.Value;

    // --- Helper Method to get Access Token ---
    private async Task<string?> GetAccessTokenAsync()
    {
        var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
        request.Content = new FormUrlEncodedContent(new[] 
        { 
            new KeyValuePair<string, string>("grant_type", "client_credentials") 
        });

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("access_token").GetString();
    }

    // --- Create Order ---
    public async Task<Result<string>> CreateOrderAsync(decimal amount, string currency)
    {
        var token = await GetAccessTokenAsync();
        if (token is null) return Error.Failure("PayPal.Auth", "Could not authenticate with PayPal.");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var orderRequest = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = currency,
                        value = amount.ToString("F2") // PayPal requires 2 decimal places
                    }
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/v2/checkout/orders", orderRequest);
        
        if (!response.IsSuccessStatusCode)
            return Error.Failure("PayPal.CreateOrder", "Failed to create PayPal order.");

        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("id").GetString()!;
    }

    // --- Capture Order ---
    public async Task<Result<bool>> CaptureOrderAsync(string transactionId)
    {
        var token = await GetAccessTokenAsync();
        if (token is null) return Error.Failure("PayPal.Auth", "Could not authenticate.");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync($"/v2/checkout/orders/{transactionId}/capture", null);
        
        if (!response.IsSuccessStatusCode) return false;

        // --- التحقق من الـ Status الفعلي ---
        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        
        // بايبال بيرجع حالة العملية في حقل اسمه status
        var status = json.RootElement.GetProperty("status").GetString();

        return status == "COMPLETED"; // لو رجع PENDING أو أي حاجة تانية هيعتبر فشل
    }
}