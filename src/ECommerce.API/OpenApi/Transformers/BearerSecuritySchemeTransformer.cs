using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models; // تأكد من وجود هذا الـ Namespace للكلاسات الأساسية

namespace ECommerce.API.OpenApi.Transformers;

internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    private const string SchemeId = JwtBearerDefaults.AuthenticationScheme;

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        
        if (!document.Components.SecuritySchemes.ContainsKey(SchemeId))
        {
            document.Components.SecuritySchemes.Add(SchemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Bearer token",
                Name = "Authorization"
            });
        }

        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // التحقق من وجود صفة [Authorize] على الـ Endpoint
        if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            // إنشاء مرجع للـ Security Scheme المعرف في الـ Components
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = SchemeId
                }
            };

            var requirement = new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            };

            operation.Security.Add(requirement);
        }

        return Task.CompletedTask;
    }
}