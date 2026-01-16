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
    AppDbContext context, UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitializeAsync()
    {
        try
        {
            // await _context.Database.EnsureCreatedAsync();
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
        // 1. إعداد الأدوار (Roles)
        var roles = new[] { nameof(Role.Admin), nameof(Role.Seller), "Customer" };
        foreach (var roleName in roles)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. إعداد مستخدم مدير (Admin) + بروفايل Customer
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

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, nameof(Role.Admin));
            }
        }

        // إنشاء بروفايل للـ Admin في جدول الـ Customers لتمكينه من العمليات التي تتطلب CustomerId
        if (!await context.Customers.AnyAsync(c => c.Email == adminEmail))
        {
            var adminProfile = Customer.Create(
                Guid.Parse(adminUser.Id),
                "System Administrator",
                "0000000000",
                adminEmail).Value;

            context.Customers.Add(adminProfile);
            await context.SaveChangesAsync();
        }

        // 3. إعداد تصنيفات أولية (Categories)
        if (!await context.Categories.AnyAsync())
        {
            var electronics = Category.Create(
                Guid.NewGuid(),
                "Electronics",
                "Gadgets and devices",
                null).Value;
            context.Categories.Add(electronics);
            await context.SaveChangesAsync();
        }

        // 4. إعداد بائع افتراضي (Seller) + بروفايل Customer
        var sellerEmail = "seller@localhost";
        var sellerUser = await userManager.FindByEmailAsync(sellerEmail);

        if (sellerUser == null)
        {
            sellerUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = sellerEmail,
                Email = sellerEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(sellerUser, "Seller123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(sellerUser, nameof(Role.Seller));
            }
        }

        // إنشاء بروفايل الـ Customer المرتبط بالبائع
        if (!await context.Customers.AnyAsync(c => c.Email == sellerEmail))
        {
            var sellerProfile = Customer.Create(
                Guid.Parse(sellerUser.Id),
                "Premium Seller",
                "1234567890",
                sellerEmail).Value;

            context.Customers.Add(sellerProfile);
            await context.SaveChangesAsync();
        }
    }

}


public static class InitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitializeAsync();

        await initializer.SeedAsync();
    }
}