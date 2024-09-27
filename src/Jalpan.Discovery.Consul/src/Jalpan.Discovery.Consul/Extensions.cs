using Consul;
using Jalpan.Discovery.Consul;
using Jalpan.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Discovery;

public static class Extensions
{
    private const string DefaultSectionName = "consul";
    private const string RegistryKey = "discovery.consul";

    public static IJalpanBuilder AddConsul(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrEmpty(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<ConsulOptions>();
        builder.Services.Configure<ConsulOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            throw new ConfigurationException("Consul URL cannot be empty.", nameof(options.Url));
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
            throw new ConfigurationException("Service URL cannot be empty.", nameof(options.Service.Url));
        }

        if (string.IsNullOrWhiteSpace(options.Service.Name))
        {
            throw new ConfigurationException("Service name cannot be empty.", nameof(options.Service.Name));
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