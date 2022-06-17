using System.Linq.Expressions;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Marten.Events.CodeGeneration;

namespace DevMikroblog.Modules.Posts.Infrastructure.Query;

using Marten.Linq;

public class GetPagedPostsQuery : ICompiledListQuery<MartenPost>
{
    public int PageSize { get; init; } = 12;
    [MartenIgnore] public int Page { get; init; } = 1;
    [MartenIgnore] public int SkipCount => (Page - 1) * PageSize;

    public Expression<Func<IMartenQueryable<MartenPost>, IEnumerable<MartenPost>>> QueryIs()
    {
        return posts => posts.OrderBy(x => x.CreatedAt).Skip(SkipCount).Take(PageSize);
    }
}

public class GetPostsInTagQuery : ICompiledListQuery<MartenPost>
{
    public int PageSize { get; init; } = 12;
    [MartenIgnore] public string Tag { get; init; } = null!;
    [MartenIgnore] public int Page { get; init; } = 1;
    [MartenIgnore] public int SkipCount => (Page - 1) * PageSize;

    public Expression<Func<IMartenQueryable<MartenPost>, IEnumerable<MartenPost>>> QueryIs()
    {
        return posts => posts.Where(x => x.Tags.Contains(Tag)).OrderBy(x => x.CreatedAt).Skip(SkipCount).Take(PageSize);
    }
}