namespace ECommerce.API.Contracts.Address;

public record AddAddressRequest(
    string Title,
    string City,
    string Street,
    string FullAddress);