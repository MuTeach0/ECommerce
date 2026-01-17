using ECommerce.Domain.Categories;
using ECommerce.Domain.Customers;
using ECommerce.Domain.Customers.Items;
using ECommerce.Domain.Customers.Reviews;
using ECommerce.Domain.Identity;
using ECommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Application.Common.Interfaces;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct);
    public DbSet<Customer> Customers { get; }
    public DbSet<Category> Categories { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<Address> Addresses { get; }
    public DbSet<ProductItem> ProductItems { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }
    public DbSet<Review> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}