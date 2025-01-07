using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Jalpan.Persistence.MongoDB;

public sealed class MongoDbHealthCheck(IMongoDatabase mongoDatabase) : IHealthCheck
{
    private readonly Command<BsonDocument> _command =
        new BsonDocumentCommand<BsonDocument>(BsonDocument.Parse("{ping:1}"));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await mongoDatabase.RunCommandAsync(_command, cancellationToken: cancellationToken).ConfigureAwait(false);
            return HealthCheckResult.Healthy(
                $"Successfully connected to database '{mongoDatabase.DatabaseNamespace.DatabaseName}'.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"Unable to connect to database '{mongoDatabase.DatabaseNamespace.DatabaseName}'.", ex);
        }
    }
}