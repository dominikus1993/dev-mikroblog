using DevMikroblog.Modules.Posts.Core.Model;

namespace DevMikroblog.Modules.Posts.Core.Providers;

public record GetPostsQuery(int? Page, int? PageSize);

public interface IPostsProvider
{
    IAsyncEnumerable<Post> Provide(GetPostsQuery query, CancellationToken cancellationToken = default);
}