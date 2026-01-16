using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Identity.DTOs;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Customers;
using ECommerce.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity;

public class IdentityService(
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService, IAppDbContext context) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IAppDbContext _context = context;

    public async Task<Result<string>> RegisterAsync(string email, string password, string fullName, string phoneNumber)
    {
        // 1. إنشاء المستخدم في الـ Identity
        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = email,
            Email = email,
            PhoneNumber = phoneNumber,
            EmailConfirmed = true
        };

        var identityResult = await _userManager.CreateAsync(user, password);

        if (!identityResult.Succeeded)
        {
            var error = identityResult.Errors.First();
            return (Result<string>)Error.Validation(error.Code, error.Description);
        }

        try
        {
            // 2. إضافة الدور (دي بتعمل SaveChanges داخلية)
            await _userManager.AddToRoleAsync(user, "Customer");

            // 3. إنشاء بروفايل العميل
            var customerResult = Customer.Create(
                Guid.Parse(user.Id),
                fullName,
                phoneNumber,
                email);

            if (customerResult.IsError)
            {
                // لو فشل الدومين لوجيك، نمسح اليوزر يدوي لضمان النظافة
                await _userManager.DeleteAsync(user);
                return (Result<string>)customerResult.Errors;
            }

            // 4. إضافة الكاستمر وعمل SaveChanges
            // الـ EF هنا هيعمل Transaction أوتوماتيك لضمان الحفظ
            _context.Customers.Add(customerResult.Value);
            await _context.SaveChangesAsync(default);

            return (Result<string>)user.Id;
        }
        catch (Exception ex)
        {
            // لو حصل أي Error في الداتا بيز، نمسح اليوزر اللي اتكريت فوق
            await _userManager.DeleteAsync(user);
            return (Result<string>)Error.Failure("Registration_Failed", ex.Message);
        }
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
    public async Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName!);
        return result.Succeeded;
    }

    public async Task<Result<AppUserDTO>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with email {UtilityService.MaskEmail(email)} not found");
        }

        if (!user.EmailConfirmed)
        {
            return Error.Conflict(
                "Email_Not_Confirmed",
                $"email '{UtilityService.MaskEmail(email)}' not confirmed");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Conflict(
                "Invalid_Login_Attempt",
                "Email / Password are incorrect");
        }

        return new AppUserDTO(user.Id, user.Email!, await _userManager.GetRolesAsync(user), await _userManager.GetClaimsAsync(user));
    }


    public async Task<Result<AppUserDTO>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ??
            throw new InvalidOperationException(nameof(userId));

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        return new AppUserDTO(user.Id, user.Email!, roles, claims);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }
}