using Jalpan.Messaging.Brokers;
using Jalpan.Messaging.RabbitMQ.Internals;
using Jalpan.Tracing.OpenTelemetry.Decorators;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Jalpan.Tracing.OpenTelemetry;

internal static class Extensions
{
    private const string ConsoleExporter = "console";
    private const string JaegerExporter = "jaeger";
    private const string SectionName = "tracing";
    private const string RegistryName = "tracing.openTelemetry";

    public static IJalpanBuilder AddTracing(this IJalpanBuilder builder, string sectionName = SectionName, string appSectionName = "app")
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<TracingOptions>();
        builder.Services.Configure<TracingOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        var appName = builder.Configuration.BindOptions<AppOptions>(appSectionName).Name;

        if (string.IsNullOrWhiteSpace(appName))
        {
            throw new InvalidOperationException("Application name cannot be empty when using the tracing.");
        }

        builder.Services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddTelemetrySdk()
                        .AddEnvironmentVariableDetector()
                        .AddService(appName))
                    .AddSource(appName)
                    .AddSource(MessageBrokerTracingDecorator.ActivitySourceName)
                    .AddSource(MessageHandlerTracingDecorator.ActivitySourceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                switch (options.Exporter.ToLowerInvariant())
                {
                    case ConsoleExporter:
                    {
                        builder.AddConsoleExporter();
                        break;
                    }
                    case JaegerExporter:
                    {
                        var jaegerOptions = options.Jaeger;
                        builder.AddJaegerExporter(jaeger =>
                        {
                            jaeger.AgentHost = jaegerOptions.AgentHost;
                            jaeger.AgentPort = jaegerOptions.AgentPort;
                            jaeger.MaxPayloadSizeInBytes = jaegerOptions.MaxPayloadSizeInBytes;
                            if (!Enum.TryParse<ExportProcessorType>(jaegerOptions.ExportProcessorType, true,
                                    out var exportProcessorType))
                            {
                                exportProcessorType = ExportProcessorType.Batch;
                            }

                            jaeger.ExportProcessorType = exportProcessorType;
                        });
                        break;
                    }
                }
            });

        return builder;
    }

    public static IServiceCollection AddMessagingTracingDecorators(this IServiceCollection services)
    {
        services.TryDecorate<IMessageBroker, MessageBrokerTracingDecorator>();
        services.TryDecorate<IMessageHandler, MessageHandlerTracingDecorator>();

        return services;
    }
}