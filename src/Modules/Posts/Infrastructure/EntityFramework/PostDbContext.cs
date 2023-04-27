using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

public sealed class PostDbContext : DbContext
{
    public DbSet<EfPost> Posts { get; set; }
    
    
    private static readonly Func<PostDbContext, PostId, CancellationToken, Task<EfPost?>> GetPostById =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id, CancellationToken ct) =>
                dbContext.Posts.FirstOrDefault(p => p.Id == id.Value));
    public Task<EfPost?> Load(PostId id, CancellationToken cancellationToken)
    {
        return GetPostById(this, id, cancellationToken);
    }
    
    public void Add(Post post)
    {
        Posts.Add(new EfPost(post));
    }
}