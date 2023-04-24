using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Marten;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public class MartenPostWriter : IPostWriter
{
    private readonly IDocumentStore _store;

    public MartenPostWriter(IDocumentStore store)
    {
        _store = store;
    }

    public async Task Save(Post post, CancellationToken cancellationToken = default)
    {
        var dbPost = new EfPost(post);
        await using var session = _store.LightweightSession();
        session.Insert(dbPost);
        await session.SaveChangesAsync(cancellationToken);
    }
}