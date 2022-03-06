using DevMikroblog.Modules.Posts.Core.Model;
using DevMikroblog.Modules.Posts.Core.Providers;

namespace DevMikroblog.Modules.Posts.Core.UseCases;

public class GetPostsUseCase
{
    private IPostsProvider _postProvider;

    public GetPostsUseCase(IPostsProvider postProvider)
    {
        _postProvider = postProvider;
    }

    public Task<Post> Execute(CancellationToken cancellationToken = default)
    {
        return _postProvider.Provide(cancellationToken).FirstAsync(cancellationToken: cancellationToken);
    }
}