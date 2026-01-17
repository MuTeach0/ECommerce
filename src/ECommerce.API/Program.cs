using ECommerce.Infrastructure.Data;
using ECommerce.Application;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Configuration
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// 2. Register Layer Services
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        // Link to the v2 document as the default for Single-Vendor
        options.WithTitle("ECommerce API V2 (Single-Vendor)")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
               // This links Scalar specifically to the V2 OpenAPI document
               .WithOpenApiRoutePattern("/openapi/v2.json");
    });

    await app.InitializeDatabaseAsync();
}
app.UseCoreMiddlewares(builder.Configuration);

// Map controllers to handle versioned routes
app.MapControllers();

app.Run();