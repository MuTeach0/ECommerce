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

            // 1. يمسك أي خطأ يحصل في الـ Pipeline كله
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));

            // 2. يسجل البيانات الأساسية
            // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>)); // لو هضيفه كـ Behavior

            // 3. يفحص الـ Validation (لو فشل مش هيفتح Transaction)
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

            // 4. يفحص الأداء
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));

            // 5. يتعامل مع الكاش للاستعلامات (Queries)
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));

            // 6. يفتح الترانزاكشن للـ Commands (آخر طبقة قبل الـ Handler)
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        return services;
    }
}