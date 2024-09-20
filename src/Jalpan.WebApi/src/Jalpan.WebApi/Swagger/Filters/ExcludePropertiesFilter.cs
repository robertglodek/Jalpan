﻿using Humanizer;
using Jalpan.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;
using System.Reflection;

namespace Jalpan.WebApi.Swagger.Filters;

internal sealed class ExcludePropertiesFilter : ISchemaFilter
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
    private static readonly ConcurrentDictionary<Type, IDictionary<string, OpenApiSchema>> Properties = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties.Count == 0)
        {
            return;
        }

        if (Properties.TryGetValue(context.Type, out var properties))
        {
            schema.Properties = properties;
            return;
        }

        var excludedProperties = context.Type
            .GetProperties(Flags)
            .Where(x => x.GetCustomAttribute<HiddenAttribute>() is not null)
            .Select(x => x.Name.Camelize());

        foreach (var excludedProperty in excludedProperties)
        {
            if (schema.Properties.ContainsKey(excludedProperty))
            {
                schema.Properties.Remove(excludedProperty);
            }
        }

        Properties.TryAdd(context.Type, schema.Properties);
    }
}
