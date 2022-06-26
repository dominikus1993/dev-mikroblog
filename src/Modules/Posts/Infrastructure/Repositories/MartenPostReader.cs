using System.Runtime.CompilerServices;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.Model;


using LanguageExt;

using Marten;
using Marten.Linq;
using Marten.Linq.Fields;
using Marten.Linq.Parsing;
using Marten.Pagination;

using static LanguageExt.Prelude;

namespace DevMikroblog.Modules.Posts.Infrastructure.Repositories;

internal class MartenPostReader : IPostsReader
{
    private readonly IDocumentStore _store;
    
    public MartenPostReader(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<Option<Post>> GetPostById(PostId postId, CancellationToken cancellationToken = default)
    {
        await using var session = _store.QuerySession();
        var result = await session.LoadAsync<MartenPost>(postId.Value, cancellationToken);
        return Optional(result).Map(post => post.MapToPost());
    }

    public async IAsyncEnumerable<Post> GetPosts(GetPostQuery query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var session = _store.QuerySession();
        IQueryable<MartenPost> q = session.Query<MartenPost>();

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
        foreach (var post in result)
        {
            yield return post.MapToPost();
        }
    }
}