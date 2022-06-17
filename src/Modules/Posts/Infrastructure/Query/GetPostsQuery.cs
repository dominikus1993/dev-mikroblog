using System.Linq.Expressions;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

namespace DevMikroblog.Modules.Posts.Infrastructure.Query;

using Marten.Linq;

internal class GetPagedPostsQuery : ICompiledListQuery<MartenPost>
{
    public int PageSize { get; init; } = 12;
    public int Page { get; init; } = 1;

    public Expression<Func<IMartenQueryable<MartenPost>, IEnumerable<MartenPost>>> QueryIs()
    {
        var skip = (Page - 1) * PageSize;
        return posts => posts.OrderBy(x => x.CreatedAt).Skip(skip).Take(PageSize);
    }
}

internal class GetPostsInTagQuery : ICompiledListQuery<MartenPost>
{
    public int PageSize { get; init; } = 12;
    public int Page { get; init; } = 1;
    public string Tag { get; init; } = null!;


    public Expression<Func<IMartenQueryable<MartenPost>, IEnumerable<MartenPost>>> QueryIs()
    {
        var skip = (Page - 1) * PageSize;
        return posts => posts.Where(x => x.Tags.Contains(Tag)).OrderBy(x => x.CreatedAt).Skip(skip).Take(PageSize);
    }
}