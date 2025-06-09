using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shop.Api;

public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            paths.Add(path.Key.Replace("v{version}", $"v{swaggerDoc.Info.Version}"), path.Value);
        }

        swaggerDoc.Paths = paths;
    }
}
