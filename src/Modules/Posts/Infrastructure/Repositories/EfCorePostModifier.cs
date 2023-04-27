using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public class EfCorePostModifier : IPostModifier
{
    private readonly PostDbContext _context;
    
    public EfCorePostModifier(PostDbContext store)
    {
        _context = store;
    }
    
    public async Task<Unit> Modify(PostId postId, Func<Post, Post> modifyF, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(modifyF);
        var post = await _context.Load(postId, cancellationToken);
        if (post is null)
        {
            return Unit.Default;
        }
        var model = modifyF.Invoke(post.MapToPost());
        
        // session.Store(new EfPost(model));
        //
        // await session.SaveChangesAsync(cancellationToken);
        return Unit.Default;
    }
}