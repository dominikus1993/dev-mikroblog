using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public class MartenPostWriter : IPostWriter
{
    public Task CreatePost(Post post, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}