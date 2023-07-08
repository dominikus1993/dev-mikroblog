using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Extensions;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

public sealed class EntityPostRepository: IPostsReader, IPostWriter
{
    
    private readonly PostDbContext _context;

    public EntityPostRepository(PostDbContext context)
    {
        _context = context;
    }
    public async Task<Post?> GetPostById(PostId postId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Load(postId, cancellationToken);
        return result;
    }

    public async Task<PostDetails?> GetPostDetails(PostId postId, CancellationToken cancellationToken = default)
    {
        var post = await _context.ReadOnlyLoad(postId, cancellationToken);
        if (post is null)
        {
            return null;
        }

        var replyTo = await _context.LoadReplyTo(post.ReplyTo, cancellationToken);

        return new PostDetails(post, replyTo);
    }

    public async Task<PagedPosts?> GetPosts(GetPostQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> q = _context.Posts.AsQueryable();

        if (query.AuthorId.HasValue)
        {
            var id = query.AuthorId;
            q = q.Where(x => x.Author.Id == id);
        }

        if (!string.IsNullOrEmpty(query.Tag))
        {
            q = q.Where(x => EF.Functions.ArrayToTsVector(x.Tags)
                .Matches(query.Tag.ToUpperInvariant()));
        }

        var result = await q.OrderByDescending(static x => x.CreatedAt).ToPagedListAsync(pageNumber: query.Page, pageSize: query.PageSize, cancellationToken);
        if (result.IsEmpty)
        {
            return null;
        }

        return new PagedPosts(result.Items, result.PageCount, result.TotalItemCount);
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