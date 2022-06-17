using DevMikroblog.Modules.Posts.Domain.Model;

using Marten.Schema;

namespace DevMikroblog.Modules.Posts.Infrastructure.Model;

public class MartenPost
{
    public Guid Id { get; init; }
    public string? Content { get; init; }
    public int Likes { get; set; }
    public Guid AuthorId { get; init; }
    public string AuthorName { get; set; }
    public DateTime CreatedAt { get; init; }
    public Guid? ReplyToPostId { get; init; }
    [FullTextIndex]
    public List<string>? Tags { get; init; }
    
    public void IncrementLikes()
    {
        Likes += 1;
    }

    public MartenPost()
    {
        
    }

    public MartenPost(Post post)
    {
        Id = post.Id.Value;
        Content = post.Content;
        Likes = post.Likes;
        AuthorId = post.Author.Id.Value;
        AuthorName = post.Author.Name;
        CreatedAt = post.CreatedAt;
        Tags = post.Tags?.Select(x => x.Value).ToList();
    }
    
    public Post MapToPost()
    {
        ReplyToPost? replyTo = ReplyToPostId.HasValue ? new ReplyToPost(new PostId(ReplyToPostId.Value)) : null;
        var tags = Tags?.Select(x => new Tag(x)).ToList();
        return new Post(new PostId(Id), Content, replyTo, CreatedAt, new Author(new AuthorId(AuthorId), AuthorName), tags,
            Likes);
    }
}