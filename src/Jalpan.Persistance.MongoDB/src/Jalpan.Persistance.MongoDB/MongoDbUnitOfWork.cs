using MongoDB.Driver;

namespace Jalpan.Persistance.MongoDB;

internal class MongoDbUnitOfWork(IMongoClient mongoClient) : IUnitOfWork
{
    private readonly IMongoClient _mongoClient = mongoClient;

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        IClientSessionHandle session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

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
        IClientSessionHandle session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

        try
        {
            session.StartTransaction();

            T result = await action();

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
