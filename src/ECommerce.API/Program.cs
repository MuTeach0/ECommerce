using ECommerce.Infrastructure.Data; // تأكد من الـ Namespace الصحيح
using ECommerce.Application;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. تسجيل السيريلوج (Logging)
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// 2. تسجيل خدمات الطبقات (هنا الربط الجوهري)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// تسجيل الطبقات الخاصة بك (تأكد أنك أنشأت الـ Extension Methods هذه)
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// 3. إعداد الـ Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        // هذا السطر يربط الواجهة بالملف المولد من MapOpenApi
        options.WithTitle("ECommerce API v1")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });

    await app.InitializeDatabaseAsync();
}
app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();

app.Run();