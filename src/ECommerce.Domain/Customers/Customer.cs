using System.Net.Mail;
using System.Text.RegularExpressions;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers.Items;
using ECommerce.Domain.Orders;

namespace ECommerce.Domain.Customers;
public sealed class Customer : AuditableEntity
{
    // أزلنا الـ ? وجعلناها string.Empty لتتوافق مع IsRequired في الـ Configuration
    public string Name { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    // الـ Backing Fields التي سيتعامل معها EF Core بناءً على الـ Configuration
    private readonly List<ProductItem> _ownedProducts = [];
    public IReadOnlyCollection<ProductItem> OwnedProducts => _ownedProducts.AsReadOnly();

    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private Customer() { } // مطلوب للـ EF Core

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
        // التحقق من البيانات قبل التحديث
        if (string.IsNullOrWhiteSpace(name)) return CustomerErrors.NameRequired;
        if (string.IsNullOrWhiteSpace(email)) return CustomerErrors.EmailRequired;
        if (string.IsNullOrWhiteSpace(phoneNumber)) return CustomerErrors.InvalidPhoneNumber;

        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;

        return Result.Updated;
    }

    public Result<Updated> UpsertOwnedProducts(List<ProductItem> incomingItem)
    {
        // حذف المنتجات غير الموجودة في القائمة الجديدة
        _ownedProducts.RemoveAll(existing => incomingItem.All(v => v.Id != existing.Id));

        foreach (var incoming in incomingItem)
        {
            var existing = _ownedProducts.FirstOrDefault(p => p.Id == incoming.Id);
            if (existing is null)
            {
                _ownedProducts.Add(incoming);
            }
            else
            {
                // استخدام الـ Update الذي يدعم الـ CostPrice
                var updateItemResult = existing.Update(
                    incoming.Name,
                    incoming.Description,
                    incoming.Price,
                    incoming.CostPrice, // تأكدنا أن الـ Configuration تدعم هذا الحقل
                    incoming.StockQuantity,
                    incoming.SKU);

                if (updateItemResult.IsError)
                {
                    return updateItemResult.Errors;
                }
            }
        }

        return Result.Updated;
    }
}