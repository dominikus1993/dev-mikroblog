using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

public sealed class PostDbContext : DbContext
{
    public DbSet<EfPost> Posts { get; set; }
    
}