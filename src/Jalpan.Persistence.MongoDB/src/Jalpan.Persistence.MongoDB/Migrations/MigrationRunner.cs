using Jalpan.Persistence.MongoDB.Migrations.Models;
using MongoDB.Driver;

namespace Jalpan.Persistence.MongoDB.Migrations;

public class MigrationRunner(IMongoDatabase db) : IMigrationRunner
{
    public async Task RunAsync(IEnumerable<MigrationBase> migrations)
    {
        var orderedMigrations = migrations.OrderBy(m => m.Timestamp);
        var appliedCollection = db.GetCollection<AppliedMigration>("__migrations");
        var applied = await appliedCollection.Find(_ => true).ToListAsync();

        foreach (var migration in orderedMigrations)
        {
            if (applied.Any(a => a.Name == migration.Name)) continue;

            Console.WriteLine($"Applying: {migration.Name}");
            await migration.Up(db);
            await appliedCollection.InsertOneAsync(new AppliedMigration
            {
                Name = migration.Name,
                AppliedAt = DateTime.UtcNow
            });
        }
    }
}
