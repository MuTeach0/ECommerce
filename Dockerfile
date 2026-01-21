# ----- مرحلة البناء (نستخدم 10 ليفهم الكود) -----
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# نسخ الملفات
COPY ["src/ECommerce.API/ECommerce.API.csproj", "src/ECommerce.API/"]
COPY ["src/ECommerce.Application/ECommerce.Application.csproj", "src/ECommerce.Application/"]
COPY ["src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "src/ECommerce.Infrastructure/"]
COPY ["src/ECommerce.Domain/ECommerce.Domain.csproj", "src/ECommerce.Domain/"]

# هنا التعديل: إجبار الـ Restore على التعامل مع نسخة 9
RUN dotnet restore "src/ECommerce.API/ECommerce.API.csproj" /p:TargetFramework=net9.0

COPY . .
WORKDIR "/src/src/ECommerce.API"

# بناء المشروع ليخرج بصيغة متوافقة مع .NET 9
RUN dotnet publish "ECommerce.API.csproj" -c Release -o /app/publish /p:TargetFramework=net9.0 /p:UseAppHost=false

# ----- المرحلة النهائية (التشغيل على 9 المستقرة) -----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# إعدادات الـ Globalization لـ SQL Server
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apt-get update && apt-get install -y icu-devtools && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]