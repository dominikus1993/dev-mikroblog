using DevMikroblog.Modules.Posts.Infrastructure.EfCore.Configuration;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EfCore;

class PostsDbContext : DbContext
{
    public DbSet<EfPost> Posts => Set<EfPost>();

    public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EfPostConfiguaration());
        base.OnModelCreating(modelBuilder);
    }
}