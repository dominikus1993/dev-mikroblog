using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Configurations;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

public sealed class PostDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; } = null!;

    public PostDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostDbContext).Assembly);
    }
    
    private static readonly Func<PostDbContext, PostId, CancellationToken, Task<Post?>> GetPostById =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id, CancellationToken ct) =>
                dbContext.Posts.AsTracking().FirstOrDefault(p => p.Id == id));
    
    private static readonly Func<PostDbContext, PostId, CancellationToken, Task<Post?>> GetPostByIdReadonly =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id, CancellationToken ct) =>
                dbContext.Posts.AsNoTracking().FirstOrDefault(p => p.Id == id));
    
    private static readonly Func<PostDbContext, PostId, IAsyncEnumerable<Post>> GetReplies =
        EF.CompileAsyncQuery(
            (PostDbContext dbContext, PostId id) =>
                dbContext.Posts.AsNoTracking().Where(p => p.ReplyTo.Id == id));
    
    public Task<Post?> Load(PostId id, CancellationToken cancellationToken)
    {
        return GetPostById(this, id, cancellationToken);
    }
    
    public Task<Post?> ReadOnlyLoad(PostId id, CancellationToken cancellationToken)
    {
        return GetPostByIdReadonly(this, id, cancellationToken);
    }
    
    public async Task<Post?> LoadReplyTo(ReplyToPost? reply, CancellationToken cancellationToken)
    {
        if (reply is null)
        {
            return null;
        }
        return await GetPostByIdReadonly(this, reply.Id, cancellationToken);
    }

    public IAsyncEnumerable<Post> GetRepliesTo(PostId id)
    {
        return GetReplies(this, id);
    }
}