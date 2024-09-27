using Humanizer;
using Jalpan.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;
using System.Reflection;

namespace Jalpan.WebApi.Swagger.Filters;

internal sealed class ExcludePropertiesFilter : ISchemaFilter
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
    private static readonly ConcurrentDictionary<Type, IDictionary<string, OpenApiSchema>> CachedProperties = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties.Count == 0)
        {
            return;
        }

        if (CachedProperties.TryGetValue(context.Type, out var cachedProperties))
        {
            schema.Properties = cachedProperties;
            return;
        }

        var excludedProperties = GetExcludedProperties(context.Type);
        RemoveExcludedProperties(schema.Properties, excludedProperties);

        CachedProperties.TryAdd(context.Type, schema.Properties);
    }

    private static IEnumerable<string> GetExcludedProperties(Type type)
    {
        return type.GetProperties(Flags)
            .Where(prop => prop.GetCustomAttribute<HiddenAttribute>() != null)
            .Select(prop => prop.Name.Camelize());
    }

    private static void RemoveExcludedProperties(IDictionary<string, OpenApiSchema> properties, IEnumerable<string> excludedProperties)
    {
        foreach (var excludedProperty in excludedProperties)
        {
            properties.Remove(excludedProperty);
        }
    }
}
