using System.Collections;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Extensions;

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int PageCount, int TotalItemCount) : IEnumerable<T>
{
    public int Count => Items.Count;

    public static readonly PagedResult<T> Empty = new(Array.Empty<T>(), 0, 0);
    
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return PagedResult<T>.Empty;
    }
}