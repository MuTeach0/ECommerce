using System.Diagnostics;
using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUser user,
    IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger = logger;
    private readonly IUser _user = user;
    private readonly IIdentityService _identityService = identityService;

    // Implementation of the performance behavior
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next(cancellationToken);

        _timer.Stop();
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if(elapsedMilliseconds > 500) // Threshold in milliseconds
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogWarning(
                "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, elapsedMilliseconds, userId, userName, request);
        }
        return response;
    }
}