using System.Runtime.CompilerServices;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using LanguageExt;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Repositories;

public record GetPostQuery(int Page, int PageSize, string? Tag = null, AuthorId? AuthorId = null);

public interface IPostsReader
{
    Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default);
    Task<Option<PagedPosts>> GetPosts(GetPostQuery query, CancellationToken cancellationToken);
}