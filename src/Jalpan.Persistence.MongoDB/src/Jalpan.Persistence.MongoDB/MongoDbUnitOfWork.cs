using MongoDB.Driver;

namespace Jalpan.Persistence.MongoDB;

internal class MongoDbUnitOfWork(IMongoClient mongoClient) : IUnitOfWork
{
    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var session = await mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

        try
        {
            session.StartTransaction();

            await action();

            await session.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var session = await mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

        try
        {
            session.StartTransaction();

            var result = await action();

            await session.CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
        finally 
        {
            session.Dispose();
        }
    }
}
