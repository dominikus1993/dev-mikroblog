using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Extensions;
using DevMikroblog.Modules.Posts.Infrastructure.Model;


using LanguageExt;

using Microsoft.EntityFrameworkCore;

using static LanguageExt.Prelude;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

internal sealed class MartenPostReader : IPostsReader
{
    private readonly IDbContextFactory<PostDbContext> _contextFactory;
    
    public MartenPostReader(IDbContextFactory<PostDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Post?> GetPostById(PostId postId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.Load(postId, cancellationToken);
        return result;
    }

    public async Task<PostDetails?> GetPostDetails(PostId postId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.Load(postId, cancellationToken);
        if (result is null)
        {
            return null;
        }

        var parent = result.ReplyTo.HasValue ?
            await context.Load(result.ReplyToPostId.Value, cancellationToken)
            : null;

        var replies = await context.GetRepliesTo(postId)
            .ToArrayAsync(cancellationToken);

        var postParent = Optional(parent).Map(x => x.MapToPost());
        var postReplies = Optional(replies).Where(x => x.Length > 0)
            .Map<IReadOnlyList<Post>>(posts => posts.Select(post => post.MapToPost()).ToList());

        return new PostDetails(postParent, result.MapToPost(), postReplies);
    }

    public async Task<Option<PagedPosts>> GetPosts(GetPostQuery query, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<Post> q = context.Posts.AsQueryable();

        if (query.AuthorId.HasValue)
        {
            var id = query.AuthorId;
            q = q.Where(x => x.Author.Id == id);
        }
        if (!string.IsNullOrEmpty(query.Tag))
        {
            q = q.Where(x => x.Tags!.Contains(query.Tag));
        }
        
        var result = await q.OrderByDescending(static x => x.CreatedAt).ToPagedListAsync(pageNumber: query.Page, pageSize: query.PageSize, cancellationToken);
        if (result.Count == 0)
        {
            return None;
        }

        var postList = result.Select(static x => x.MapToPost()).ToArray();

        return Some(new PagedPosts(postList, result.PageCount, result.TotalItemCount));
    }
}