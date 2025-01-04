using System.Reflection;
using Jalpan.Messaging.Brokers;
using Jalpan.Metrics.OpenTelemetry.Decorators;
using Jalpan.Metrics.OpenTelemetry.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;

namespace Jalpan.Metrics.OpenTelemetry;

public static class Extensions
{
    private const string ConsoleExporter = "console";
    private const string PrometheusExporter = "prometheus";
    private const string DefaultSectionName = "metrics";
    private const string RegistryKey = "metrics.openTelemetry";

    public static IJalpanBuilder AddMetrics(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<MetricsOptions>();
        builder.Services.Configure<MetricsOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        builder.Services.AddOpenTelemetry()
            .WithMetrics(configure =>
            {
                configure.AddAspNetCoreInstrumentation();
                configure.AddHttpClientInstrumentation();
                configure.AddRuntimeInstrumentation();

                foreach (var attribute in GetMeterAttributes())
                {
                    if (attribute is not null)
                    {
                        configure.AddMeter(attribute.Key);
                    }
                }

                ConfigureExporter(configure, sectionName, options);
            });

        return builder;
    }

    private static void ConfigureExporter(MeterProviderBuilder builder, string sectionName, MetricsOptions options)
    {
        if (string.IsNullOrEmpty(options.Exporter))
        {
            throw new MetricsConfigurationException("Metrics explorer cannot be empty.");
        }

        switch (options.Exporter.ToLowerInvariant())
        {
            case ConsoleExporter:
                builder.AddConsoleExporter();
                break;
            case PrometheusExporter:
                builder.AddPrometheusExporter(prometheus =>
                    prometheus.ScrapeEndpointPath = string.IsNullOrWhiteSpace(options.Endpoint)
                        ? prometheus.ScrapeEndpointPath
                        : options.Endpoint);
                break;
            default:
                throw new MetricsConfigurationException($"Metrics explorer '{options.Exporter}' not configured.");
        }
    }

    public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
    {
        var metricsOptions = app.ApplicationServices.GetRequiredService<IOptions<MetricsOptions>>().Value;
        if (!metricsOptions.Enabled)
        {
            return app;
        }

        if (metricsOptions.Exporter.ToLowerInvariant() is not PrometheusExporter)
        {
            return app;
        }

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        return app;
    }

    private static IEnumerable<MeterAttribute?> GetMeterAttributes()
        => AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic)
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && x.GetCustomAttribute<MeterAttribute>() is not null)
            .Select(x => x.GetCustomAttribute<MeterAttribute>())
            .Where(x => x is not null);

    public static IJalpanBuilder AddMessagingMetricsDecorators(this IJalpanBuilder builder)
    {
        builder.Services.TryDecorate<IMessageBroker, MessageBrokerMetricsDecorator>();

        return builder;
    }
}