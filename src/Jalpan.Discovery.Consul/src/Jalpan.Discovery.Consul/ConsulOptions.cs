namespace Jalpan.Discovery.Consul;

public sealed class ConsulOptions
{
    public bool Enabled { get; init; }
    public string Url { get; init; } = string.Empty;
    public ServiceRegistrationOptions Service { get; init; } = new();
    public HealthCheckRegistrationOptions HealthCheck { get; init; } = new();

    public sealed class ServiceRegistrationOptions
    {
        public string Name { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = [];
    }

    public sealed class HealthCheckRegistrationOptions
    {
        public string Endpoint { get; init; } = string.Empty;
        public TimeSpan? Interval { get; init; }
        public TimeSpan? DeregisterInterval { get; init; }
    }
}