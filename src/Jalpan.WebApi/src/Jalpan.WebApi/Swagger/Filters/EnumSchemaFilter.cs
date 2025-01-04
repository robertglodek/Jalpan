using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Jalpan.WebApi.Swagger.Filters;

[UsedImplicitly]
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;
        schema.Type = "string";

        var camelCaseNames = Enum.GetNames(context.Type)
            .Select(name => new OpenApiString(JsonNamingPolicy.CamelCase.ConvertName(name))).ToList<IOpenApiAny>();

        schema.Enum = camelCaseNames;
    }
}