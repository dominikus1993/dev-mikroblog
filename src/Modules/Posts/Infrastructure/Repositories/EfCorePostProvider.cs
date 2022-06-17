using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EfCore;

using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

internal class EfCorePostProvider : IPostsReader
{
    private readonly PostsDbContext _postsDbContext;

    public EfCorePostProvider(PostsDbContext postsDbContext)
    {
        _postsDbContext = postsDbContext;
    }

    public async Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default)
    {
        var result = await _postsDbContext.Posts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == postId.Value, cancellationToken);
        return Optional(result).Map(post => post.MapToPost());
    }

    public IAsyncEnumerable<Post> GetPosts(GetPostQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
}