using ECommerce.Domain.Categories;
using ECommerce.Domain.Customers;
using ECommerce.Domain.Identity;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Data;

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    AppDbContext context,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitializeAsync()
    {
        try
        {
            // Apply pending migrations to the database
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // 1. Seed Roles: Only Admin and Customer are needed for Single Vendor system
        var roles = new[] { nameof(Role.Admin), "Customer" };
        foreach (var roleName in roles)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Seed Default Admin User
        var adminEmail = "admin@ecommerce.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            // Create admin with a secure password
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                // Assign Admin role to the user
                await userManager.AddToRoleAsync(adminUser, nameof(Role.Admin));
            }
        }

        // 3. Seed Admin Customer Profile 
        // We create a Customer entity linked to the Admin User ID for business consistency
        if (!await context.Customers.AnyAsync(c => c.Email == adminEmail))
        {
            var adminProfileResult = Customer.Create(
                Guid.Parse(adminUser.Id),
                "System Administrator",
                "0000000000",
                adminEmail);

            if (adminProfileResult.IsSuccess)
            {
                context.Customers.Add(adminProfileResult.Value);
                await context.SaveChangesAsync();
            }
        }

        // 4. Seed Initial Categories if empty
        if (!await context.Categories.AnyAsync())
        {
            var electronicsResult = Category.Create(
                Guid.NewGuid(),
                "Electronics",
                "Gadgets and devices",
                null);

            if (electronicsResult.IsSuccess)
            {
                context.Categories.Add(electronicsResult.Value);
                await context.SaveChangesAsync();
            }
        }

        // Note: Seller seeding was removed as we migrated to a Single Vendor architecture.
    }
}

public static class InitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        // Run migrations first
        await initializer.InitializeAsync();

        // Then seed initial data
        await initializer.SeedAsync();
    }
}