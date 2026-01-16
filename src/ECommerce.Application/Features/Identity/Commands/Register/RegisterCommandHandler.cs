using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Identity.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken ct)
    {
        // بينادي الميثود اللي لسه معدلينها في الـ IdentityService
        return await identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName,
            request.PhoneNumber);
    }
}