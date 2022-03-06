using DevMikroblog.Modules.Posts.Core.Model;

namespace DevMikroblog.Modules.Posts.Core.Providers;

public interface IPostsProvider
{
    IAsyncEnumerable<Post> Provide(CancellationToken cancellationToken = default);
}