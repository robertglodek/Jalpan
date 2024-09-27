using Figgle;
using Jalpan.Serializers;
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
    private const string DefaultSectionName = "app";

    public static IServiceCollection AddJalpan(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IJalpanBuilder>? configure = null,
        string sectionName = DefaultSectionName)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

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

        services.AddHostedService<StartupInitializer>();

        configure?.Invoke(builder);

        if (string.IsNullOrWhiteSpace(options.Name))
        {
            return services;
        }

        var version = !string.IsNullOrWhiteSpace(options.Version) ? $" {options.Version}" : string.Empty;
        Console.WriteLine(FiggleFonts.Doom.Render($"{options.Name}{version}"));

        return services;
    }

    public static IServiceCollection AddInitializer<T>(this IServiceCollection services) where T : class, IInitializer
        => services.AddTransient<IInitializer, T>();

    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }

    public static string Underscore(this string value)
       => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLowerInvariant();
}
