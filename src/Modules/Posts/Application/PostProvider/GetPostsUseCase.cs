using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Repositories;

namespace DevMikroblog.Modules.Posts.Application.PostProvider;

internal record GetPostQuery(int Page, int PageSize);

internal class GetPostsUseCase
{
    private readonly IPostsReader _postProvider;

    public GetPostsUseCase(IPostsReader postProvider)
    {
        _postProvider = postProvider;
    }

    public async Task<IReadOnlyList<PostDto>> Execute(GetPostQuery query, CancellationToken cancellationToken = default)
    {
        return await _postProvider.GetPosts(new Domain.Repositories.GetPostQuery(query.Page, query.PageSize), cancellationToken).Select(x => PostDto.FromPost(x)).ToListAsync(cancellationToken);
    }
}