using System.Net.Mail;
using System.Text.RegularExpressions;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;
using ECommerce.Domain.Orders;

namespace ECommerce.Domain.Customers;
public sealed class Customer : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;


    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private Customer() { }

    private Customer(Guid id, string name, string phoneNumber, string email)
        : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
    }
    public Result<Success> AddAddress(string title, string city, string street, string fullAddress)
    {
        var addressResult = Address.Create(Id, title, city, street, fullAddress);
        if (addressResult.IsError) return addressResult.Errors;

        _addresses.Add(addressResult.Value);
        return Result.Success;
    }

    public static Result<Customer> Create(Guid id, string name, string phoneNumber, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) return CustomerErrors.NameRequired;
        if (string.IsNullOrWhiteSpace(email)) return CustomerErrors.EmailRequired;

        try
        {
            _ = new MailAddress(email);
        }
        catch
        {
            return CustomerErrors.EmailInvalid;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
        {
            return CustomerErrors.InvalidPhoneNumber;
        }

        return new Customer(id, name, phoneNumber, email);
    }

    public Result<Updated> Update(string name, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name)) return CustomerErrors.NameRequired;
        if (string.IsNullOrWhiteSpace(email)) return CustomerErrors.EmailRequired;
        if (string.IsNullOrWhiteSpace(phoneNumber)) return CustomerErrors.InvalidPhoneNumber;

        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;

        return Result.Updated;
    }
}