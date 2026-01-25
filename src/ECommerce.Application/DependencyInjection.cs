using System.Reflection;
using ECommerce.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>)); // لو هضيفه كـ Behavior

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));

            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));

            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}