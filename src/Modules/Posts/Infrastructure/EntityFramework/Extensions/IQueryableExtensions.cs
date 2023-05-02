using System.Collections;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Extensions;

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int PageCount, int TotalItemCount) : IEnumerable<T>
{
    public int Count => Items.Count;
    public bool IsEmpty => Items.Count == 0;

    public static readonly PagedResult<T> Empty = new(Array.Empty<T>(), 0, 0);
    
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class QueryableExtensions
{
    public static Task<PagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "PageNumber should be grater than 0");
        }
        
        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "PageSize should be grater than 0");
        }

        return ToPagedList(query, pageNumber, pageSize, cancellationToken);
    }

    private static async Task<PagedResult<T>> ToPagedList<T>(this IQueryable<T> query, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var rowCount = await query.CountAsync(cancellationToken: cancellationToken);

        if (rowCount == 0)
        {
            return PagedResult<T>.Empty;
        }
        
        var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);

        var skip = (pageNumber - 1) * pageSize;
        
        var result = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);
        
        return new PagedResult<T>(result, pageCount, rowCount);
    }
}