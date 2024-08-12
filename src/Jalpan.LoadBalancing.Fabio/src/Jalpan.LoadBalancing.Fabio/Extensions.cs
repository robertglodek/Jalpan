using Consul;
using Micro.HTTP.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.LoadBalancing.Fabio;

public static class Extensions
{
    private const string SectionName = "fabio";
    private const string RegistryName = "loadBalancing.fabio";

    public static IJalpanBuilder AddFabio(this IJalpanBuilder builder, string sectionName = SectionName, string consulSectionName = "consul")
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<FabioOptions>();
        builder.Services.Configure<FabioOptions>(section);

        return builder.AddFabio(options, b => b.AddConsul(consulSectionName));
    }

    private static IJalpanBuilder AddFabio(this IJalpanBuilder builder, FabioOptions options, Action<IJalpanBuilder> registerConsul)
    {
        registerConsul(builder);
        builder.Services.AddSingleton(options);

        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            throw new ArgumentException("Fabio URL cannot be empty.", nameof(options.Url));
        }

        builder.Services.AddTransient<FabioHttpHandler>();
        builder.UpdateConsulRegistration();

        return builder;
    }

    private static void UpdateConsulRegistration(this IJalpanBuilder builder)
    {
        AgentServiceRegistration registration;
        using (var serviceProvider = builder.Services.BuildServiceProvider())
        {
            registration = serviceProvider.GetRequiredService<AgentServiceRegistration>();
        }

        var tags = GetFabioTags(registration.Name);
        if (registration.Tags is null)
        {
            registration.Tags = [.. tags];
        }
        else
        {
            registration.Tags = [.. registration.Tags, .. tags];
        }

        var serviceDescriptor = builder.Services.FirstOrDefault(sd => sd.ServiceType == typeof(AgentServiceRegistration));
        builder.Services.Remove(serviceDescriptor!);
        builder.Services.AddSingleton(registration);
    }

    private static List<string> GetFabioTags(string consulServiceName) => [$"urlprefix-/{consulServiceName} strip=/{consulServiceName}"];

    public static IHttpClientBuilder AddFabioHandler(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<FabioHttpHandler>();
}