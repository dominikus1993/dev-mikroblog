using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using LanguageExt;

using Marten;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public class MartenPostModifier : IPostModifier
{
    private readonly IDocumentStore _store;
    
    public MartenPostModifier(IDocumentStore store)
    {
        _store = store;
    }
    
    public async Task<Unit> Modify(PostId postId, Func<Post, Post> modifyF, CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        var post = await session.LoadAsync<MartenPost>(postId.Value, cancellationToken);
        if (post is null)
        {
            return Unit.Default;
        }
        var model = modifyF.Invoke(post.MapToPost());
        
        session.Store(new MartenPost(model));

        await session.SaveChangesAsync(cancellationToken);
        return Unit.Default;
    }
}