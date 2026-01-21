using ECommerce.Infrastructure.Data;
using ECommerce.Application;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog - يقرأ تلقائياً من appsettings.json
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// 2. Register Layer Services
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// 3. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Map both v1 and v2

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("ECommerce API V2 (Single-Vendor)")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
               .WithOpenApiRoutePattern("/openapi/v2.json"); // الربط بـ V2
    });

    // تهيئة قاعدة البيانات (Migrations & Seeding)
    await app.InitializeDatabaseAsync();
}

app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();

app.Run();