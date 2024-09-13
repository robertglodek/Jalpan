using Jalpan.GatewayApi.Auth;
using Jalpan.GatewayApi.Configuration;
using Jalpan.GatewayApi.Handlers;
using Jalpan.GatewayApi.Requests;
using Jalpan.GatewayApi.Routing;
using Jalpan.GatewayApi.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;

namespace Jalpan.GatewayApi;

public static class Extensions
{
    private const string SectionName = "gateway";

    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        var configuration = BuildConfiguration(services);

        return services.AddCoreServices()
            .ConfigureLogging(configuration)
            .ConfigureHttpClient(configuration)
            .ConfigurePayloads(configuration)
            .AddNtradaServices();
    }

    private static GatewayOptions BuildConfiguration(IServiceCollection services)
    {
        IConfiguration config;
        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            config = scope.ServiceProvider.GetService<IConfiguration>();
        }

        var section = config.GetSection(SectionName);
        var options = section.BindOptions<GatewayOptions>();
        services.Configure<GatewayOptions>(section);

        return options;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddMvcCore()
            .AddNewtonsoftJson(o => o.SerializerSettings.Formatting = Formatting.Indented)
            .AddApiExplorer();
        
        return services;
    }
    
    private static IServiceCollection ConfigureLogging(this IServiceCollection services, GatewayOptions options)
    {
        services.AddLogging(
            builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole();
            });
        
        return services;
    }

    private static IServiceCollection ConfigureHttpClient(this IServiceCollection services, GatewayOptions options)
    {
        var http = options.Http ?? new Http();
        var httpClientBuilder = services.AddHttpClient("jalpan.gateway");
        httpClientBuilder.AddTransientHttpErrorPolicy(p =>
            p.WaitAndRetryAsync(http.Retries, retryAttempt =>
            {
                var interval = http.Exponential
                    ? Math.Pow(http.Interval, retryAttempt)
                    : http.Interval;

                return TimeSpan.FromSeconds(interval);
            }));


        var httpClientbuilder = services
           .AddHttpClient("jalpan.gateway.api.httpClient")
           .AddTransientHttpErrorPolicy(_ => HttpPolicyExtensions.HandleTransientHttpError()
               .WaitAndRetryAsync(options.Resiliency.Retries, retry =>
                   options.Resiliency.Exponential
                       ? TimeSpan.FromSeconds(Math.Pow(2, retry))
                       : options.Resiliency.RetryInterval ?? TimeSpan.FromSeconds(2)));




        return services;
    }

    private static IServiceCollection ConfigurePayloads(this IServiceCollection services, GatewayOptions options)
    {
        if (options.PayloadsFolder is null)
        {
            options.PayloadsFolder = "Payloads";
        }

        if (options.PayloadsFolder.EndsWith("/"))
        {
            options.PayloadsFolder = options.PayloadsFolder
                .Substring(0, options.PayloadsFolder.Length - 1);
        }
        
        return services;
    }

    private static IServiceCollection AddNtradaServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationManager, AuthenticationManager>();
        services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
        services.AddSingleton<IPolicyManager, PolicyManager>();
        services.AddSingleton<IDownstreamBuilder, DownstreamBuilder>();
        services.AddSingleton<IPayloadBuilder, PayloadBuilder>();
        services.AddSingleton<IPayloadManager, PayloadManager>();
        services.AddSingleton<IPayloadTransformer, PayloadTransformer>();
        services.AddSingleton<IPayloadValidator, PayloadValidator>();
        services.AddSingleton<IRequestExecutionValidator, RequestExecutionValidator>();
        services.AddSingleton<IRequestHandlerManager, RequestHandlerManager>();
        services.AddSingleton<IRequestProcessor, RequestProcessor>();
        services.AddSingleton<IRouteConfigurator, RouteConfigurator>();
        services.AddSingleton<IRouteProvider, RouteProvider>();
        services.AddSingleton<ISchemaValidator, SchemaValidator>();
        services.AddSingleton<IUpstreamBuilder, UpstreamBuilder>();
        services.AddSingleton<IValueProvider, ValueProvider>();
        services.AddSingleton<DownstreamHandler>();
        services.AddSingleton<ReturnValueHandler>();
        services.AddSingleton<WebApiEndpointDefinitions>();
        
        return services;
    }
    
    public static IApplicationBuilder UseNtrada(this IApplicationBuilder app)
    {
        //var newLine = Environment.NewLine;
        var logger = app.ApplicationServices.GetRequiredService<ILogger<JalpanGateway>>();
        //logger.LogInformation($"{newLine}{newLine}{Logo}{newLine}{newLine}");
        var options = app.ApplicationServices.GetRequiredService<GatewayOptions>();
        if (options.Auth?.Enabled == true)
        {
            logger.LogInformation("Authentication is enabled.");
            app.UseAuthentication();
        }
        else
        {
            logger.LogInformation("Authentication is disabled.");
        }

        if (options.UseForwardedHeaders)
        {
            logger.LogInformation("Headers forwarding is enabled.");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
        }

        if (options.LoadBalancer?.Enabled == true)
        {
            logger.LogInformation($"Load balancer is enabled: {options.LoadBalancer.Url}");
        }
        
        app.RegisterRequestHandlers();
        app.AddRoutes();

        return app;
    }

    private static void RegisterRequestHandlers(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<JalpanGateway>>();
        var options = app.ApplicationServices.GetRequiredService<GatewayOptions>();
        var requestHandlerManager = app.ApplicationServices.GetRequiredService<IRequestHandlerManager>();
        requestHandlerManager.AddHandler("downstream",
                 app.ApplicationServices.GetRequiredService<DownstreamHandler>());
        requestHandlerManager.AddHandler("return_value",
            app.ApplicationServices.GetRequiredService<ReturnValueHandler>());

        if (options.Modules is null)
        {
            return;
        }

        var handlers = options.Modules
            .Select(m => m.Value)
            .SelectMany(m => m.Routes)
            .Select(r => r.Use)
            .Distinct()
            .ToArray();

        foreach (var handler in handlers)
        {
            if (requestHandlerManager.Get(handler) is null)
            {
                throw new Exception($"Handler: '{handler}' was not defined.");
            }
            
            logger.LogInformation($"Added handler: '{handler}'");
        }
    }

    private static void AddRoutes(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<GatewayOptions>();
        if (options.Modules is null)
        {
            return;
        }
        
        foreach (var route in options.Modules.SelectMany(m => m.Value.Routes))
        {
            if (route.Methods is {})
            {
                if (route.Methods.Any(m => m.Equals(route.Method, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ArgumentException($"There's already a method {route.Method.ToUpperInvariant()} declared in route 'methods', as well as in 'method'.");
                }
                
                continue;
            }

            route.Method = (string.IsNullOrWhiteSpace(route.Method) ? "get" : route.Method).ToLowerInvariant();
            route.DownstreamMethod =
                (string.IsNullOrWhiteSpace(route.DownstreamMethod) ? route.Method : route.DownstreamMethod)
                .ToLowerInvariant();
        }

        var routeProvider = app.ApplicationServices.GetRequiredService<IRouteProvider>();
        app.UseRouting();
        app.UseEndpoints(routeProvider.Build());
    }
    
    private class JalpanGateway
    {
    }

    public static IApplicationBuilder UseRequestHandler<T>(this IApplicationBuilder app, string name)
        where T : IHandler
    {
        var requestHandlerManager = app.ApplicationServices.GetRequiredService<IRequestHandlerManager>();
        var handler = app.ApplicationServices.GetRequiredService<T>();
        requestHandlerManager.AddHandler(name, handler);

        return app;
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.IsEmpty();

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable is null || !enumerable.Any();

    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
       => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }
}