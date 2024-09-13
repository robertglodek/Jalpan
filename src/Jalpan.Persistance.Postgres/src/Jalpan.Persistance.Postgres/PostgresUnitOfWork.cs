using Microsoft.EntityFrameworkCore;

namespace Jalpan.Persistance.Postgres;

internal sealed class PostgresUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    public PostgresUnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await action();
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            T result = await action();
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
}