using System.Collections.Concurrent;

namespace Jalpan;

public sealed class JalpanBuilder : IJalpanBuilder
{
    private readonly ConcurrentDictionary<string, bool> _registry = new();
    public IServiceCollection Services { get; }
    public IConfiguration Configuration { get; }

    private JalpanBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    public static IJalpanBuilder Create(IServiceCollection services, IConfiguration configuration)
        => new JalpanBuilder(services, configuration);

    public bool TryRegister(string name) 
        => _registry.TryAdd(name, true);
}
