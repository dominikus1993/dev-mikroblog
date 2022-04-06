using System.Runtime.CompilerServices;

using DevMikroblog.Modules.Posts.Infrastructure.EfCore.Configuration;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
namespace DevMikroblog.Modules.Posts.Infrastructure.EfCore;

public class PostsDbContext : DbContext
{
    public DbSet<EfPost> Posts { get; set; } = null!;

    public PostsDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EfPostConfiguaration());
        base.OnModelCreating(modelBuilder);
    }
}