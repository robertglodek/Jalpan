using Jalpan.Pagination;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Jalpan.Persistence.MongoDB;

public static class Pagination
{
    public static async Task<PagedResult<T>> PaginateAsync<T>(this IMongoQueryable<T> collection, IPagedQuery query, CancellationToken cancellationToken = default)
        => await collection.PaginateAsync(query.OrderBy, query.SortOrder, query.Page, query.Results, cancellationToken);

    public static async Task<PagedResult<T>> PaginateAsync<T>(this IMongoQueryable<T> collection, string? orderBy,
        SortOrder? sortOrder, int page, int results, CancellationToken cancellationToken = default)
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

        var totalResults = await collection.CountAsync(cancellationToken);
        if(totalResults == 0)
        {
            return PagedResult<T>.Empty;
        }

        var totalPages = (int)Math.Ceiling((decimal)totalResults / results);

        List<T> result;
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            result = await collection.Limit(page, results).ToListAsync(cancellationToken);
            return PagedResult<T>.Create(result, page, results, totalPages, totalResults);
        }

        if (sortOrder is null or SortOrder.Asc)
        {
            result = await collection.OrderBy(ToLambda<T>(orderBy)).Limit(page, results).ToListAsync(cancellationToken);
        }
        else
        {
            result = await collection.OrderByDescending(ToLambda<T>(orderBy)).Limit(page, results).ToListAsync(cancellationToken);
        }

        return PagedResult<T>.Create(result, page, results, totalPages, totalResults);
    }

    public static IMongoQueryable<T> Limit<T>(this IMongoQueryable<T> collection,
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
