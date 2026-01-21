using ECommerce.Application.Common.Interfaces;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Common.Behaviors;

/// <summary>
/// A pipeline behavior that logs every request sent through MediatR.
/// It captures user information and the request data.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUser user,
    IIdentityService identityService)
    : IPipelineBehavior<TRequest, TResponse> // 1. Updated to implement IPipelineBehavior
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly IUser _user = user;
    private readonly IIdentityService _identityService = identityService;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        // 2. Fetching user info from the identity service if a User ID exists in the context
        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        // 3. Logging the request details BEFORE the handler executes
        _logger.LogInformation(
            "ECommerce Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        // 4. Proceeding to the next behavior in the pipeline or the actual request handler
        var response = await next();

        // 5. You can optionally log successful completion here
        _logger.LogInformation("ECommerce Request {Name} handled successfully", requestName);

        return response;
    }
}