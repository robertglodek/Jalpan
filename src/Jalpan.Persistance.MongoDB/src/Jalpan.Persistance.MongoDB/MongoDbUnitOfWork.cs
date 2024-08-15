using Jalpan.Persistance.MongoDB.Factories;
using MongoDB.Driver;

namespace Jalpan.Persistance.MongoDB;

internal class MongoDbUnitOfWork : IUnitOfWork
{
    private readonly IMongoDbSessionFactory _mongoDbSessionFactory;

    public MongoDbUnitOfWork(IMongoDbSessionFactory mongoDbSessionFactory)
    {
        _mongoDbSessionFactory = mongoDbSessionFactory;
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        IClientSessionHandle session = await _mongoDbSessionFactory.CreateAsync();

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
        IClientSessionHandle session = await _mongoDbSessionFactory.CreateAsync();

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
    }
}
