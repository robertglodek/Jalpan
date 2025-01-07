using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jalpan.WebApi;

public static class Extensions
{
    private const string DefaultSelfHealthCheckName = "self_health_check";
    public static IHealthChecksBuilder AddSelfCheck(this IHealthChecksBuilder builder, string name = DefaultSelfHealthCheckName)
        => builder.AddCheck(name, () => HealthCheckResult.Healthy());
}