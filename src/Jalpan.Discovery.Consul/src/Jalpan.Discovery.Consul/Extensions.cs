using Consul;
using Jalpan;
using Jalpan.Discovery.Consul;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.HTTP.ServiceDiscovery;

public static class Extensions
{
    private const string SectionName = "consul";
    private const string RegistryName = "discovery.consul";

    public static IJalpanBuilder AddConsul(this IJalpanBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<ConsulOptions>();
        builder.Services.Configure<AppOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            throw new ArgumentException("Consul URL cannot be empty.", nameof(options.Url));
        }

        builder.Services.AddTransient<ConsulHttpHandler>();
        builder.Services.AddHostedService<ConsulRegistrationService>();
        builder.Services.AddSingleton<IConsulClient>(new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri(options.Url);
        }));

        builder.Services.AddSingleton(builder.CreateConsulAgentRegistration(options));

        return builder;
    }

    private static AgentServiceRegistration CreateConsulAgentRegistration(this IJalpanBuilder builder, ConsulOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Service.Url))
        {
            throw new ArgumentException("Service URL cannot be empty.", nameof(options.Service.Url));
        }

        if (string.IsNullOrWhiteSpace(options.Service.Name))
        {
            throw new ArgumentException("Service name cannot be empty.", nameof(options.Service.Name));
        }

        string serviceId;
        using (var serviceProvider = builder.Services.BuildServiceProvider())
        {
            serviceId = serviceProvider.GetRequiredService<IServiceId>().Id;
        }

        var serviceUrl = new Uri(options.Service.Url);

        return new AgentServiceRegistration
        {
            ID = $"{options.Service.Name}:{serviceId}",
            Name = options.Service.Name,
            Address = serviceUrl.Host,
            Port = serviceUrl.Port,
            Tags = [.. options.Service.Tags],
            Check = new AgentServiceCheck
            {
                HTTP = $"{serviceUrl}{options.HealthCheck.Endpoint}",
                Interval = options.HealthCheck.Interval,
                DeregisterCriticalServiceAfter = options.HealthCheck.DeregisterInterval
            }
        };
    }

    public static IHttpClientBuilder AddConsulHandler(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<ConsulHttpHandler>();
}