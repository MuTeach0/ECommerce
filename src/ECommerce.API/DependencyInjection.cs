
using System.Text.Json.Serialization;
using Asp.Versioning;
using ECommerce.API.OpenApi.Transformers;
using ECommerce.API.Services;
using ECommerce.Application.Common.Interfaces;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // استخدام الأسلوب الخاص بك (Method Chaining)
        services
            .AddCustomProblemDetails()
            .AddCustomApiVersioning()   // تم حل مشكلة الـ Route Constraint هنا
            .AddApiDocumentation()      // إعدادات الـ Swagger / OpenApi
            .AddControllerWithJsonConfiguration()
            .AddIdentityInfrastructure()
            .AddAppOutputCaching();

        return services;
    }

    public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            // إخبار النظام أن الإصدار يأتي في مسار الـ URL
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddMvc() // ضروري لربط الـ Constraints مع نظام الـ Routing
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        string[] versions = ["v1"];

        foreach (var version in versions)
        {
            services.AddOpenApi(version, options =>
            {
                // Versioning config
                options.AddDocumentTransformer<VersionInfoTransformer>();

                // Security Scheme config
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
            });
        }

        return services;
    }

    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options => 
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        return services;
    }

    public static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        return services;
    }
    public static IServiceCollection AddAppOutputCaching(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(60)));
        });
        return services;
    }
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUser, CurrentUser>();
        return services;
    }

    // ترتيب الميدل وير كما في المشروع القديم
    public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();
        
        // app.UseCors(...); // فعلها لو عندك CorsPolicy
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseOutputCache();

        return app;
    }
}