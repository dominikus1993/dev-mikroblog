using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.EfCore;

class PostsDbContext : DbContext
{
    public DbSet<EfPost> Posts { get; set; }
}