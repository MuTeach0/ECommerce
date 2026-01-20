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

        services
            .AddCustomProblemDetails()
            .AddCustomApiVersioning()
            .AddApiDocumentation()
            .AddControllerWithJsonConfiguration()
            .AddIdentityInfrastructure()
            .AddAppOutputCaching();

        // إضافة دعم الـ CORS بناءً على الـ appsettings
        services.AddCors(options =>
        {
            var allowedOrigins = configuration.GetSection("AppSettings:AllowedOrigins").Get<string[]>();
            options.AddPolicy(configuration["AppSettings:CorsPolicyName"] ?? "ECommercePolicy",
                policy => policy.WithOrigins(allowedOrigins ?? ["*"])
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });

        return services;
    }

    public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(2, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        // تم تفعيل V2 كنسخة أساسية بناءً على طلبك لـ Scalar
        string[] versions = ["v1", "v2"];

        foreach (var version in versions)
        {
            services.AddOpenApi(version, options =>
            {
                options.AddDocumentTransformer<VersionInfoTransformer>();
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

    public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();

        // تفعيل الـ CORS
        app.UseCors(configuration["AppSettings:CorsPolicyName"] ?? "ECommercePolicy");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseOutputCache();

        return app;
    }
}