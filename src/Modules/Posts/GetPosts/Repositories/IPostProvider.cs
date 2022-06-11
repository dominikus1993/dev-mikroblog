using DevMikroblog.Modules.Posts.Core.Model;
using LanguageExt;

namespace DevMikroblog.Modules.Posts.GetPosts.Repositories;

internal record GetPostQuery(int Page, int PageSize);

internal interface IPostsReader
{
    Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Post> GetPosts(GetPostQuery query, CancellationToken cancellationToken);
}