using Jalpan.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Jalpan.Persistence.Postgres;

public static class Pagination
{
    public static Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> data, IPagedQuery query,
        CancellationToken cancellationToken = default)
        => data.PaginateAsync(query.OrderBy, query.SortOrder, query.Page, query.Results, cancellationToken);

    public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> data, string? orderBy,
        string? sortOrder, int page, int results, CancellationToken cancellationToken = default)
    {
        if (page <= 0)
        {
            page = 1;
        }

        results = results switch
        {
            <= 0 => 10,
            > 100 => 100,
            _ => results
        };

        var totalResults = await data.CountAsync(cancellationToken);
        var totalPages = totalResults <= results ? 1 : (int)Math.Floor((double)totalResults / results);

        List<T> result;
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            result = await data.Skip((page - 1) * results).Take(results).ToListAsync(cancellationToken);
            return PagedResult<T>.Create(result, page, results, totalPages, totalResults);
        }

        if (sortOrder?.ToLowerInvariant() == "asc")
        {
            if (typeof(T).GetProperty(orderBy) != null)
            {
                result = await data.OrderBy(ToLambda<T>(orderBy)).Limit(page, results).ToListAsync(cancellationToken);
            }
            else
            {
                result = await data.Limit(page, results).ToListAsync(cancellationToken);
            }
        }
        else
        {
            if (typeof(T).GetProperty(orderBy) != null)
            {
                result = await data.OrderByDescending(ToLambda<T>(orderBy)).Limit(page, results).ToListAsync(cancellationToken);
            }
            else
            {
                result = await data.Limit(page, results).ToListAsync(cancellationToken);
            }
        }

        return PagedResult<T>.Create(result, page, results, totalPages, totalResults);
    }

    public static IQueryable<T> Limit<T>(this IQueryable<T> collection,
        int page, int resultsPerPage)
    {
        var skip = (page - 1) * resultsPerPage;
        var data = collection.Skip(skip)
            .Take(resultsPerPage);

        return data;
    }

    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
    }
}