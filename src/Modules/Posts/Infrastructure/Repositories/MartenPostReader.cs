using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Model;


using LanguageExt;

using static LanguageExt.Prelude;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

internal class MartenPostReader : IPostsReader
{
    private readonly PostDbContext _store;
    
    public MartenPostReader(PostDbContext store)
    {
        _store = store;
    }

    public async Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken)
    {
        var result = await _store.Load(postId, cancellationToken);
        return Optional(result).Map(static post => post.MapToPost());
    }

    public async Task<Option<PostDetails>> GetPostDetails(PostId postId, CancellationToken cancellationToken)
    {
        var result = await _store.Load(postId, cancellationToken);
        if (result is null)
        {
            return None;
        }

        var parent = result.ReplyToPostId.HasValue ?
            await _store.Load(result.ReplyToPostId.Value, cancellationToken)
            : null;

        var replies = await _store.GetRepliesTo(postId)
            .ToArrayAsync(cancellationToken);

        var postParent = Optional(parent).Map(x => x.MapToPost());
        var postReplies = Optional(replies).Where(x => x.Length > 0)
            .Map<IReadOnlyList<Post>>(posts => posts.Select(post => post.MapToPost()).ToList());

        return new PostDetails(postParent, result.MapToPost(), postReplies);
    }

    public async Task<Option<PagedPosts>> GetPosts(GetPostQuery query, CancellationToken cancellationToken)
    {
        await using var session = _store.QuerySession();
        IQueryable<EfPost> q = session.Query<EfPost>();

        if (query.AuthorId.HasValue)
        {
            var id = query.AuthorId.Value;
            q = q.Where(x => x.AuthorId == id.Value);
        }
        if (!string.IsNullOrEmpty(query.Tag))
        {
            q = q.Where(x => x.Tags!.Contains(query.Tag));
        }
        
        var result = await q.OrderByDescending(x => x.CreatedAt).ToPagedListAsync(pageNumber: query.Page, pageSize: query.PageSize, cancellationToken);
        if (result.Count == 0)
        {
            return None;
        }

        var postList = result.Select(x => x.MapToPost()).ToList();

        return Some(new PagedPosts(postList, result.PageCount, result.TotalItemCount));
    }
}