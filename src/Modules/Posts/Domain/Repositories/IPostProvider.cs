using DevMikroblog.Modules.Posts.Domain.Model;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Domain.Repositories;

internal record GetPostQuery(int Page, int PageSize);

internal interface IPostsReader
{
    Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Post> GetPosts(GetPostQuery query, CancellationToken cancellationToken);
}