using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Domain.Repositories;

internal interface IPostWriter
{
    Task CreatePost(Post post, CancellationToken cancellationToken = default);
}