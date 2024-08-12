using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Jalpan.Handlers;
using Jalpan.Logging.Serilog.Middlewares;
using Jalpan.Logging.Serilog.Decorators;

namespace Jalpan.Logging.Serilog;

public static class Extensions
{
    private const string LoggerSectionName = "logger";
    private const string AppSectionName = "app";
    internal static LoggingLevelSwitch LoggingLevelSwitch = new();
    private const string ConsoleOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}";
    private const string FileOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";

    public static IJalpanBuilder AddLogger(this IJalpanBuilder builder, string loggerSectionName = LoggerSectionName)
    {
        if (string.IsNullOrWhiteSpace(loggerSectionName))
        {
            loggerSectionName = LoggerSectionName;
        }

        var section = builder.Configuration.GetSection(loggerSectionName);
        builder.Services.Configure<LoggerOptions>(section);

        builder.Services.AddSingleton<ContextLoggingMiddleware>();
        builder.Services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));

        return builder;
    }

    public static IApplicationBuilder UseContextLogger(this IApplicationBuilder app)
       => app.UseMiddleware<ContextLoggingMiddleware>();

    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder,
        Action<HostBuilderContext, LoggerConfiguration>? configure = null,
        string loggerSectionName = LoggerSectionName,
        string appSectionName = AppSectionName) => hostBuilder
            .ConfigureServices(services => services.AddSingleton<ILoggingService, LoggingService>())
            .UseSerilog((context, loggerConfiguration) =>
            {
                if (string.IsNullOrWhiteSpace(loggerSectionName))
                {
                    loggerSectionName = LoggerSectionName;
                }

                if (string.IsNullOrWhiteSpace(appSectionName))
                {
                    appSectionName = AppSectionName;
                }

                var appOptions = context.Configuration.BindOptions<AppOptions>(appSectionName);
                var loggerOptions = context.Configuration.BindOptions<LoggerOptions>(loggerSectionName);

                MapOptions(loggerOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
                configure?.Invoke(context, loggerConfiguration);
            });
    private static void MapOptions(LoggerOptions loggerOptions, AppOptions appOptions,
        LoggerConfiguration loggerConfiguration,
        string environmentName)
    {
        LoggingLevelSwitch.MinimumLevel = GetLogEventLevel(loggerOptions.Level);

        loggerConfiguration.Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", appOptions.Service)
            .Enrich.WithProperty("Instance", appOptions.Instance)
            .Enrich.WithProperty("Version", appOptions.Version);

        foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
        {
            loggerConfiguration.Enrich.WithProperty(key, value);
        }

        foreach (var (key, value) in loggerOptions.MinimumLevelOverrides ?? new Dictionary<string, string>())
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

        loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));

        Configure(loggerConfiguration, loggerOptions);
    }

    private static void Configure(LoggerConfiguration loggerConfiguration,
        LoggerOptions options)
    {
        var consoleOptions = options.Console ?? new LoggerOptions.ConsoleOptions();
        var fileOptions = options.File ?? new LoggerOptions.FileOptions();
        var seqOptions = options.Seq ?? new LoggerOptions.SeqOptions();
        var mongoOptions = options.Mongo ?? new LoggerOptions.MongoDBOptions();

        if (consoleOptions.Enabled)
        {
            var template = !string.IsNullOrEmpty(consoleOptions.Template) ? consoleOptions.Template : ConsoleOutputTemplate;
            loggerConfiguration.WriteTo.Console(outputTemplate: template);
        }

        if (fileOptions.Enabled)
        {
            var template = !string.IsNullOrEmpty(fileOptions.Template) ? fileOptions.Template : FileOutputTemplate;
            var path = string.IsNullOrWhiteSpace(fileOptions.Path) ? "logs/logs.txt" : fileOptions.Path;
            if (!Enum.TryParse<RollingInterval>(fileOptions.Interval, true, out var interval))
            {
                interval = RollingInterval.Day;
            }

            loggerConfiguration.WriteTo.File(path, rollingInterval: interval);
        }

        if (seqOptions.Enabled)
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
        }

        if (mongoOptions.Enabled)
        {
            loggerConfiguration.WriteTo.MongoDBBson(cfg =>
            {
                cfg.SetMongoUrl(mongoOptions.Url);
                cfg.SetCollectionName(mongoOptions.Collection);
            });
        } 
    }

    public static IEndpointConventionBuilder MapLogLevelHandler(this IEndpointRouteBuilder builder,
        string endpointRoute = "~/logging/level")
        => builder.MapPost(endpointRoute, LevelSwitch);

    internal static LogEventLevel GetLogEventLevel(string? level)
        => Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;

    private static async Task LevelSwitch(HttpContext context)
    {
        var service = context.RequestServices.GetService<ILoggingService>();
        if (service is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("ILoggingService is not registered. Add UseLogging() to your Program.cs.");
            return;
        }

        var level = context.Request.Query["level"].ToString();

        if (string.IsNullOrEmpty(level))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid value for logging level.");
            return;
        }

        service.SetLoggingLevel(level);

        context.Response.StatusCode = StatusCodes.Status200OK;
    }
}
