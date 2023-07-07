using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Extensions;

using Microsoft.EntityFrameworkCore;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

internal sealed class MartenPostReader : IPostsReader
{
    private readonly PostDbContext _context;

    public MartenPostReader(PostDbContext context)
    {
        _context = context;
    }


    public async Task<Post?> GetPostById(PostId postId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Load(postId, cancellationToken);
        return result;
    }

    public Task<PostDetails?> GetPostDetails(PostId postId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<PostDetails?>(null);
    }

    public async Task<PagedPosts?> GetPosts(GetPostQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> q = _context.Posts.AsQueryable();

        if (query.AuthorId.HasValue)
        {
            var id = query.AuthorId;
            q = q.Where(x => x.Author.Id == id);
        }
        
        // if (!string.IsNullOrEmpty(query.Tag))
        // {
        //     q = q.Where(x => x.Tags!.Contains(query.Tag));
        // }
        
        var result = await q.OrderByDescending(static x => x.CreatedAt).ToPagedListAsync(pageNumber: query.Page, pageSize: query.PageSize, cancellationToken);
        if (result is null or { Count: 0 })
        {
            return null;
        }

        return new PagedPosts(result.Items, result.PageCount, result.TotalItemCount);
    }
}