using Jalpan.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Jalpan.WebApi.CORS;

public static class Extensions
{
    private const string SectionName = "cors";
    private const string RegistryName = "webApi.cors";
    private const string PolicyName = "cors";

    public static IJalpanBuilder AddCorsPolicy(this IJalpanBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<CorsOptions>();
        builder.Services.Configure<CorsOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryName))
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
                    throw new ConfigurationException("Cannot use wildcard '*' with AllowCredentials enabled.", nameof(options.AllowCredentials));
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
