using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Application.PostProvider;

internal record GetPostsQuery(int Page, int PageSize, string? Tag = null, AuthorId? AuthorId = null);
public record PagedPostsDto(IReadOnlyList<PostDto> Posts, long TotalPages, long TotalPostsQuantity);

internal class GetPostsUseCase
{
    private readonly IPostsReader _postProvider;

    public GetPostsUseCase(IPostsReader postProvider)
    {
        _postProvider = postProvider;
    }

    public async Task<PagedPostsDto?> Execute(GetPostsQuery query, CancellationToken cancellationToken = default)
    {
        var result =
            await _postProvider.GetPosts(new GetPostQuery(query.Page, query.PageSize, query.Tag, query.AuthorId),
                cancellationToken);
        if (result is null)
        {
            return null;
        }
        
        var dtos = result.Posts.Select(static post => PostDto.FromPost(post)).ToList();
        return new PagedPostsDto(dtos, result.TotalPages,
            result.TotalPostsQuantity);
    }
}