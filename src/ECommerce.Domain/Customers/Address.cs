using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Customers;

public sealed class Address : AuditableEntity
{
    public string Title { get; private set; } // مثال: المنزل، العمل
    public string City { get; private set; }
    public string Street { get; private set; }
    public string FullAddress { get; private set; }
    public Guid CustomerId { get; private set; }

    private Address() 
    {
        Title = null!;
        City = null!;
        Street = null!;
        FullAddress = null!;
    }
    private Address(Guid id, Guid customerId, string title, string city, string street, string fullAddress)
        : base(id)
    {
        CustomerId = customerId;
        Title = title;
        City = city;
        Street = street;
        FullAddress = fullAddress;
    }

    public static Result<Address> Create(Guid customerId, string title, string city, string street, string fullAddress)
    {
        if (customerId == Guid.Empty)
            return Error.Validation("Address.InvalidCustomer", "Customer ID is required.");

        if (string.IsNullOrWhiteSpace(title))
            return Error.Validation("Address.TitleRequired", "Title (e.g., Home, Work) is required.");

        if (string.IsNullOrWhiteSpace(city))
            return Error.Validation("Address.CityRequired", "City is required.");

        if (string.IsNullOrWhiteSpace(fullAddress))
            return Error.Validation("Address.Required", "Full address details are required.");

        // لو كل شيء سليم، بنكريت الأوبجكت في الميموري بأمان
        return new Address(Guid.NewGuid(), customerId, title, city, street, fullAddress);
    }
}