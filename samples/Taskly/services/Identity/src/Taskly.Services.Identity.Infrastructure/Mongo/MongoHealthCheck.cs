using Jalpan.Persistence.MongoDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Taskly.Services.Identity.Infrastructure.Mongo;

public sealed class MongoHealthCheck(IOptions<MongoDbOptions> options) : IHealthCheck
{
    public const string Name = "Mongo health check";

    private readonly Command<BsonDocument> _command =
        new BsonDocumentCommand<BsonDocument>(BsonDocument.Parse("{ping:1}"));

    private readonly MongoDbOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var mongoClient = new MongoClient(_options.ConnectionString);

            await mongoClient.GetDatabase(_options.Database)
                .RunCommandAsync(_command, cancellationToken: cancellationToken).ConfigureAwait(false);
            return HealthCheckResult.Healthy($"{Name} success.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{Name} failure.", ex);
        }
    }
}