namespace Jalpan.Persistence.MongoDB.Migrations;

public interface IMigrationRunner
{
    Task RunAsync(IEnumerable<MigrationBase> migrations);
}
