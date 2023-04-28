using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Model;
namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public sealed class MartenPostWriter : IPostWriter
{
    private readonly PostDbContext _store;

    public MartenPostWriter(PostDbContext store)
    {
        _store = store;
    }

    public async Task Save(Post post, CancellationToken cancellationToken = default)
    {
        var dbPost = new EfPost(post);
        _store.Add(dbPost);
        await _store.SaveChangesAsync(cancellationToken);
    }
}