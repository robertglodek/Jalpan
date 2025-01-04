using Microsoft.Extensions.DependencyInjection;

namespace Taskly.Services.Identity.Infrastructure.Mongo;

public static class Extensions
{
    public static IHealthChecksBuilder AddMongoCheck(this IHealthChecksBuilder builder)
        => builder.AddCheck<MongoHealthCheck>(MongoHealthCheck.Name);
}