﻿using Jalpan.Exceptions;
using Jalpan.Messaging.Brokers;
using Jalpan.Messaging.RabbitMQ.Internals;
using Jalpan.Tracing.OpenTelemetry.Decorators;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Jalpan.Tracing.OpenTelemetry;

public static class Extensions
{
    private const string ConsoleExporter = "console";
    private const string JaegerExporter = "jaeger";
    private const string DefaultSectionName = "tracing";
    private const string DefaultAppSectionName = "app";
    private const string RegistryKey = "tracing.openTelemetry";

    public static IJalpanBuilder AddTracing(this IJalpanBuilder builder, string sectionName = DefaultSectionName,
        string appSectionName = DefaultAppSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;
        appSectionName = string.IsNullOrWhiteSpace(appSectionName) ? DefaultAppSectionName : appSectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<TracingOptions>();
        builder.Services.Configure<TracingOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        var appOptions = builder.Configuration.BindOptions<AppOptions>(appSectionName);

        if (string.IsNullOrWhiteSpace(appOptions.Name))
        {
            throw new AppConfigurationException("Application name cannot be empty when using the tracing.");
        }

        builder.Services.AddOpenTelemetry()
            .WithTracing(configure =>
            {
                configure.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddTelemetrySdk()
                        .AddEnvironmentVariableDetector()
                        .AddService(appOptions.Name))
                    .AddSource(appOptions.Name)
                    .AddSource(MessageBrokerTracingDecorator.ActivitySourceName)
                    .AddSource(MessageHandlerTracingDecorator.ActivitySourceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                switch (options.Exporter.ToLowerInvariant())
                {
                    case ConsoleExporter:
                    {
                        configure.AddConsoleExporter();
                        break;
                    }
                    case JaegerExporter:
                    {
                        var jaegerOptions = options.Jaeger;
                        configure.AddJaegerExporter(jaeger =>
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

    public static IJalpanBuilder AddMessagingTracingDecorators(this IJalpanBuilder builder)
    {
        builder.Services.TryDecorate<IMessageBroker, MessageBrokerTracingDecorator>();
        builder.Services.TryDecorate<IMessageHandler, MessageHandlerTracingDecorator>();

        return builder;
    }
}