using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Jalpan.Persistence.Redis;

public sealed class RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = connectionMultiplexer.GetDatabase();
            var result = await db.PingAsync();

            return result.TotalMilliseconds < 1000
                ? HealthCheckResult.Healthy($"Redis is responding with a latency of {result.TotalMilliseconds}ms.")
                : HealthCheckResult.Degraded($"Redis is responding but with high latency: {result.TotalMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unhealthy.", ex);
        }
    }
}