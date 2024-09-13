using Jalpan.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Jalpan.Persistance.Postgres.Repositories;

public class PostgresRepository<TEntity, TIdentifiable> : IPostgresRepository<TEntity, TIdentifiable> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public PostgresRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetAsync(TIdentifiable id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default)
        => await GetAsync(e => EF.Property<TIdentifiable>(e, "Id")!.Equals(id), include, cancellationToken);

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (include != null)
        {
            query = include(query);
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (include != null)
        {
            query = include(query);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery pagedQuery, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default) where TQuery : IPagedQuery
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (include != null)
        {
            query = include(query);
        }

        return await query.PaginateAsync(pagedQuery, cancellationToken);
    }

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<TEntity> list) => await _dbSet.AddRangeAsync(list);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void UpdateRangeAsync(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
}
