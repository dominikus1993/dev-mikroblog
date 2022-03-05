using DevMikroblog.Modules.Posts.Core.Model;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Core.Repositories;

public interface IPostsReader
{
    Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default);
}

public interface IPostsWriter
{

}

public interface IPostsRepository: IPostsReader, IPostsWriter
{

}