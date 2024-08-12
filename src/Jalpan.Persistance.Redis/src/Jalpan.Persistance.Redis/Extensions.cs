using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Jalpan.Persistance.Redis;

public static class Extensions
{
    private const string SectionName = "redis";
    private const string RegistryName = "persistence.redis";

    public static IJalpanBuilder AddRedis(this IJalpanBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<RedisOptions>();
        builder.Services.Configure<RedisOptions>(section);

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
