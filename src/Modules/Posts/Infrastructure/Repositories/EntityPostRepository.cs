using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public sealed class EntityPostRepository: IPostsReader, IPostWriter
{
    
    private readonly PostDbContext _context;

    public EntityPostRepository(PostDbContext context)
    {
        _context = context;
    }
    public async Task<Post?> GetPostById(PostId postId, CancellationToken cancellationToken)
    {
        var result = await _context.Load(postId, cancellationToken);
        return result;
    }

    public Task<PostDetails?> GetPostDetails(PostId postId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PagedPosts?> GetPosts(GetPostQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task Add(Post post, CancellationToken cancellationToken = default)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Post post, CancellationToken cancellationToken = default)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);
    }
}