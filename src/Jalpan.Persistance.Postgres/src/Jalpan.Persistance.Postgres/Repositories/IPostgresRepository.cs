using Jalpan.Pagination;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Jalpan.Persistance.Postgres.Repositories;

public interface IPostgresRepository<TEntity, TIdentifiable> where TEntity : class
{
    Task<TEntity?> GetAsync(
        TIdentifiable id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> BrowseAsync<TQuery>(
        Expression<Func<TEntity, bool>> predicate, TQuery pagedQuery,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default) where TQuery : IPagedQuery;

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> list);

    void Update(TEntity entity);

    void UpdateRangeAsync(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);
        
    void DeleteRange(IEnumerable<TEntity> entities);

    Task<int> SaveAsync();
}
