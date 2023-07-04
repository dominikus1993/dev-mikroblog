using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public sealed class MartenPostWriter : IPostWriter
{
    private readonly IDbContextFactory<PostDbContext> _contextFactory;
    
    public MartenPostWriter(IDbContextFactory<PostDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(Post post, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Posts.Add(post);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Post post, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Posts.Update(post);
        await context.SaveChangesAsync(cancellationToken);
    }
}