﻿using Jalpan.WebApi.CORS.Exceptions;
using Microsoft.AspNetCore.Builder;

namespace Jalpan.WebApi.CORS;

public static class Extensions
{
    private const string DefaultSectionName = "cors";
    private const string RegistryKey = "webApi.cors";
    private const string PolicyName = "cors";

    public static IJalpanBuilder AddCorsPolicy(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<CorsOptions>();
        builder.Services.Configure<CorsOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        builder.Services.AddCors(cors =>
        {
            var allowedHeaders = options.AllowedHeaders;
            var allowedMethods = options.AllowedMethods;
            var allowedOrigins = options.AllowedOrigins;
            var exposedHeaders = options.ExposedHeaders;
            cors.AddPolicy(PolicyName, corsBuilder =>
            {
                var origins = allowedOrigins?.ToArray() ?? [];

                // Check if AllowCredentials is true and if any origin is a wildcard
                if (options.AllowCredentials && origins.Contains("*"))
                {
                    throw new CorsConfigurationException("Cannot use wildcard '*' with AllowCredentials enabled.");
                }

                if (options.AllowCredentials)
                {
                    corsBuilder.AllowCredentials();
                }
                else
                {
                    corsBuilder.DisallowCredentials();
                }

                corsBuilder
                    .WithHeaders(allowedHeaders?.ToArray() ?? [])
                    .WithMethods(allowedMethods?.ToArray() ?? [])
                    .WithOrigins([.. origins])
                    .WithExposedHeaders(exposedHeaders?.ToArray() ?? []);
            });
        });

        return builder;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<CorsOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }

        app.UseCors(PolicyName);

        return app;
    }
}
