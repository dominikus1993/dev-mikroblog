using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Configurations;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

public sealed class PostDbContext : DbContext
{
    public DbSet<EfPost> Posts { get; set; } = null!;

    public PostDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PostConfiguration());
    }
    
    private static readonly Func<PostDbContext, PostId, CancellationToken, Task<EfPost?>> GetPostById =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id, CancellationToken ct) =>
                dbContext.Posts.AsNoTracking().FirstOrDefault(p => p.Id == id));
    
    private static readonly Func<PostDbContext, PostId, CancellationToken, Task<EfPost?>> GetPostByIdReadonly =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id, CancellationToken ct) =>
                dbContext.Posts.AsNoTracking().FirstOrDefault(p => p.Id == id));
    
    private static readonly Func<PostDbContext, PostId, IAsyncEnumerable<EfPost>> GetReplies =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id) =>
                dbContext.Posts.AsNoTracking().Where(p => p.ReplyToPostId == id));
    
    public Task<EfPost?> Load(PostId id, CancellationToken cancellationToken)
    {
        return GetPostById(this, id, cancellationToken);
    }
    
    public Task<EfPost?> ReadOnlyLoad(PostId id, CancellationToken cancellationToken)
    {
        return GetPostByIdReadonly(this, id, cancellationToken);
    }

    public IAsyncEnumerable<EfPost> GetRepliesTo(PostId id)
    {
        return GetReplies(this, id);
    }

    public void Add(Post post)
    {
        Posts.Add(new EfPost(post));
    }
}