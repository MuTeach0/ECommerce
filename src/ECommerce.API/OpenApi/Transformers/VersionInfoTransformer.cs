using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ECommerce.API.OpenApi.Transformers;

internal sealed class VersionInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var version = context.DocumentName;

        document.Info.Version = version;
        document.Info.Title = $"ECommerce API {version}";
        document.Info.Description = "Advanced E-Commerce API using .NET 9 and Clean Architecture.";

        return Task.CompletedTask;
    }
}