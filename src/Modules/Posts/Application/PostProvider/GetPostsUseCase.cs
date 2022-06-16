using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Repositories;

namespace DevMikroblog.Modules.Posts.Application.PostProvider;

internal record GetPostsQuery(int Page, int PageSize);

internal class GetPostsUseCase
{
    private readonly IPostsReader _postProvider;

    public GetPostsUseCase(IPostsReader postProvider)
    {
        _postProvider = postProvider;
    }

    public async Task<IReadOnlyList<PostDto>> Execute(GetPostsQuery query, CancellationToken cancellationToken = default)
    {
        return await _postProvider.GetPosts(new GetPostQuery(query.Page, query.PageSize), cancellationToken).Select(x => PostDto.FromPost(x)).ToListAsync(cancellationToken);
    }
}