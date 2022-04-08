using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Core.Model;
using DevMikroblog.Modules.Posts.Core.Providers;

namespace DevMikroblog.Modules.Posts.Core.UseCases;

public class GetPostsUseCase
{
    private readonly IPostsProvider _postProvider;

    public GetPostsUseCase(IPostsProvider postProvider)
    {
        _postProvider = postProvider;
    }

    public async Task<IEnumerable<PostDto>> Execute(GetPostsQuery query, CancellationToken cancellationToken = default)
    {
        return await _postProvider.Find(query, cancellationToken).Select(x => PostDto.FromPost(x)).ToListAsync(cancellationToken);
    }
}