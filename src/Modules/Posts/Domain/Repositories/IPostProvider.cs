using System.Runtime.CompilerServices;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using LanguageExt;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Repositories;

public record GetPostQuery(int Page, int PageSize, string? Tag = null, AuthorId? AuthorId = null);
public record PagedPosts(IReadOnlyCollection<Post> Posts, long TotalPages, long TotalPostsQuantity);
public record GetPostReplies(PostId PostId, int Page, int PageSize);
public record PostDetails(Option<Post> ReplyTo, Post Post, Option<IReadOnlyList<Post>> Replies);

public interface IPostsReader
{
    Task<Post?> GetPostById(PostId postId, CancellationToken cancellationToken = default);
    Task<PostDetails?> GetPostDetails(PostId postId, CancellationToken cancellationToken = default);
    Task<PagedPosts?> GetPosts(GetPostQuery query, CancellationToken cancellationToken = default);
}