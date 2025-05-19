using MongoDB.Bson;

namespace Jalpan.Persistence.MongoDB.Migrations.Models;

public sealed class AppliedMigration
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime AppliedAt { get; set; }
}
