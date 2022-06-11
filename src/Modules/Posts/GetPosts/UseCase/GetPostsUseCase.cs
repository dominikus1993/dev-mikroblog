using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.GetPosts.Repositories;

namespace DevMikroblog.Modules.Posts.GetPosts.UseCase;

internal record GetPostQuery(int Page, int PageSize);

internal class GetPostsUseCase
{
    private readonly IPostsReader _postProvider;

    public GetPostsUseCase(IPostsReader postProvider)
    {
        _postProvider = postProvider;
    }

    public async Task<IEnumerable<PostDto>> Execute(GetPostQuery query, CancellationToken cancellationToken = default)
    {
        return await _postProvider.GetPosts(new Repositories.GetPostQuery(query.Page, query.PageSize), cancellationToken).Select(x => PostDto.FromPost(x)).ToListAsync(cancellationToken);
    }
}