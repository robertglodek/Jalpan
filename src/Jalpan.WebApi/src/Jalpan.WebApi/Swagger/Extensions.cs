﻿using Jalpan.Exceptions;
using Jalpan.Helpers;
using Jalpan.WebApi.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jalpan.WebApi.Swagger;

public static class Extensions
{
    private const string DefaultSectionName = "swagger";
    private const string RegistryKey = "webApi.swagger";

    public static IJalpanBuilder AddSwaggerDocs(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<SwaggerOptions>();
        builder.Services.Configure<SwaggerOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        ValidateSwaggerOptions(options, sectionName);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            ConfigureSwaggerGen(c, options);
        });

        return builder;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;
        if (!options.Enabled)
        {
            return builder;
        }

        var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? string.Empty : options.RoutePrefix;

        builder.UseSwagger(c =>
        {
            c.RouteTemplate = $"{routePrefix}/{{documentName}}/swagger.json";
        });

        return options.ReDocEnabled
            ? builder.UseReDoc(c =>
            {
                c.RoutePrefix = routePrefix;
                c.SpecUrl = $"{options.Name}/swagger.json";
            })
            : builder.UseSwaggerUI(c =>
            {
                c.RoutePrefix = routePrefix;
                c.SwaggerEndpoint($"/{routePrefix}/{options.Name}/swagger.json".FormatEmptyRoutePrefix(), options.Title);
            });
    }

    private static void ConfigureSwaggerGen(SwaggerGenOptions options, SwaggerOptions swaggerOptions)
    {
        options.SchemaFilter<ExcludePropertiesFilter>();
        options.EnableAnnotations();
        options.CustomSchemaIds(x => x.FullName);
        options.SwaggerDoc(swaggerOptions.Name, new OpenApiInfo { Title = swaggerOptions.Title, Version = swaggerOptions.Version });

        if (swaggerOptions.IncludeSecurity)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
        }
    }

    private static void ValidateSwaggerOptions(SwaggerOptions options, string sectionName)
    {
        if (string.IsNullOrWhiteSpace(options.Name))
        {
            throw new ConfigurationException("Name cannot be null or whitespace.", PropertyPathHelper.GetOptionsPropertyPath(sectionName, nameof(options.Name)));
        }

        if (string.IsNullOrWhiteSpace(options.Title))
        {
            throw new ConfigurationException("Title cannot be null or whitespace.", PropertyPathHelper.GetOptionsPropertyPath(sectionName, nameof(options.Title)));
        }

        if (string.IsNullOrWhiteSpace(options.Version))
        {
            throw new ConfigurationException("Version cannot be null or whitespace.", PropertyPathHelper.GetOptionsPropertyPath(sectionName, nameof(options.Version)));
        }
    }

    private static string FormatEmptyRoutePrefix(this string route) => route.Replace("//", "/");
}