using Jalpan.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Jalpan.Persistence.Redis;

public static class Extensions
{
    private const string DefaultSectionName = "redis";
    private const string RegistryKey = "persistence.redis";

    public static IJalpanBuilder AddRedis(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<RedisOptions>();
        builder.Services.Configure<RedisOptions>(section);

        if(string.IsNullOrEmpty(options.ConnectionString))
        {
            throw new ConfigurationException("Redis connection string cannot be empty.", nameof(options.ConnectionString));
        }

        builder.Services
            .AddSingleton(options)
            .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(options.ConnectionString))
            .AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase(options.Database))
            .AddStackExchangeRedisCache(o =>
            {
                o.Configuration = options.ConnectionString;
                o.InstanceName = options.Instance;
            });

        return builder;
    }
}
