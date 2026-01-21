using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Categories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Customers;
using ECommerce.Domain.Customers.Items;
using ECommerce.Domain.Customers.Reviews;
using ECommerce.Domain.Identity;
using ECommerce.Domain.Orders;
using ECommerce.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) :
    IdentityDbContext<AppUser>(options), IAppDbContext
{

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductItem> ProductItems => Set<ProductItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Review> Reviews => Set<Review>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct)
    {
        return await Database.BeginTransactionAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();


        foreach (var entity in domainEntities)
        {
            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}