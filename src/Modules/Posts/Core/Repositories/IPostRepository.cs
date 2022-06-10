using DevMikroblog.Modules.Posts.Core.Model;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Core.Repositories;

public record class GetPostQuery(int Page, int PageSize);

public interface IPostsReader
{
    Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Post> GetPosts(GetPostQuery query, CancellationToken cancellationToken);
}

public interface IPostsWriter
{

}

public interface IPostsRepository: IPostsReader, IPostsWriter
{

}