using DevMikroblog.Modules.Posts.Core.Model;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Core.Providers;

public record GetPostsQuery(int? Page, int? PageSize, string? Tag);

public record GetPostQuery(string Slug);

public interface IPostsProvider
{
    IAsyncEnumerable<Post> Find(GetPostsQuery query, CancellationToken cancellationToken = default);
    Task<Option<Post>> FindOne(GetPostQuery query, CancellationToken cancellationToken = default);
}