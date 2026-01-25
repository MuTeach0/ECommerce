namespace ECommerce.Infrastructure.Settings;

public class PayPalSettings
{
    public const string SectionName = "PayPal";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Environment { get; set; } = "Sandbox"; // Sandbox or Live
}