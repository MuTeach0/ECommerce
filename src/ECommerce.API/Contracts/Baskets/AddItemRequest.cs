namespace ECommerce.API.Contracts.Baskets;

public sealed record AddItemRequest(Guid ProductId, int Quantity);