using Figgle;
using Jalpan.Serializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Jalpan.Time;
using Microsoft.AspNetCore.Http.Json;
using Jalpan.Serialization;

namespace Jalpan;

public static class Extensions
{
    private const string SectionName = "app";

    public static IJalpanBuilder AddJalpan(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = SectionName)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }
            
        var builder = JalpanBuilder.Create(services, configuration);

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<AppOptions>();
        services.Configure<AppOptions>(section);

        services.AddMemoryCache();
        services.AddSingleton<IServiceId, ServiceId>();
        services.AddSingleton<IDateTime, UtcDateTime>();
        services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        services.Configure<JsonOptions>(jsonOptions =>
        {
            jsonOptions.SerializerOptions.PropertyNameCaseInsensitive = true;
            jsonOptions.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

        if (string.IsNullOrWhiteSpace(options.Name))
        {
            return builder;
        }

        var version = !string.IsNullOrWhiteSpace(options.Version) ? $" {options.Version}" : string.Empty;
        Console.WriteLine(FiggleFonts.Doom.Render($"{options.Name}{version}"));

        return builder;
    }

    public static IApplicationBuilder UseJalpan(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
        Task.Run(initializer.InitializeAsync).GetAwaiter().GetResult();

        return app;
    }

    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }

    public static string Underscore(this string value)
       => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
           .ToLowerInvariant();
}
