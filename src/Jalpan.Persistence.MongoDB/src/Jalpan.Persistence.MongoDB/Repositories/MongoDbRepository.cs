using Jalpan.Pagination;
using Jalpan.Types;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Jalpan.Persistence.MongoDB.Repositories;

internal class MongoDbRepository<TEntity, TIdentifiable>(IMongoDatabase database, string collectionName) : IMongoDbRepository<TEntity, TIdentifiable>
    where TEntity : IIdentifiable<TIdentifiable>
{
    public IMongoCollection<TEntity> Collection { get; } = database.GetCollection<TEntity>(collectionName);

    public async Task<TEntity?> GetAsync(
        TIdentifiable id,
        CancellationToken cancellationToken = default)
        => await GetAsync(e => e.Id!.Equals(id), cancellationToken);

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await Collection.Find(predicate).SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await Collection.Find(predicate).ToListAsync(cancellationToken);

    public Task<PagedResult<TEntity>> BrowseAsync<TQuery>(
        Expression<Func<TEntity, bool>> predicate,
        TQuery query, CancellationToken cancellationToken = default) where TQuery : IPagedQuery
        => Collection.AsQueryable().Where(predicate).PaginateAsync(query, cancellationToken: cancellationToken);

    public Task AddAsync(
        TEntity entity,
        InsertOneOptions? options = null,
        CancellationToken cancellationToken = default)
        => Collection.InsertOneAsync(entity, options, cancellationToken);

    public Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
        => UpdateAsync(entity, e => e.Id!.Equals(entity.Id), cancellationToken);

    public Task UpdateAsync(
        TEntity entity,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Collection.ReplaceOneAsync(predicate, entity, cancellationToken: cancellationToken);

    public Task DeleteAsync(
        TIdentifiable id,
        CancellationToken cancellationToken = default)
        => DeleteAsync(e => e.Id!.Equals(id), cancellationToken);

    public Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Collection.DeleteOneAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Collection.Find(predicate).AnyAsync(cancellationToken);

    public Task<bool> ExistsAsync(
        TIdentifiable id,
        CancellationToken cancellationToken = default)
        => ExistsAsync(e => e.Id!.Equals(id), cancellationToken);
}
