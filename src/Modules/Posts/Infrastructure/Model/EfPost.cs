using System.ComponentModel.DataAnnotations;

using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Infrastructure.Model;

public class EfPost
{
    public PostId Id { get; init; }
    public string Content { get; init; }
    public uint Likes { get; set; }
    public AuthorId AuthorId { get; init; }
    public string? AuthorName { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public PostId? ReplyToPostId { get; init; }
    public uint RepliesQuantity { get; set; }
    
    [Timestamp]
    public uint Version { get; set; }

    public string[]? Tags { get; init; }
    
    public void IncrementLikes()
    {
        Likes += 1;
    }
    
    public void IncrementReplies()
    {
        RepliesQuantity += 1;
    }

    public EfPost()
    {
        
    }

    public EfPost(Post post)
    {
        Id = post.Id;
        Content = post.Content;
        Likes = post.Likes;
        AuthorId = post.Author.Id;
        AuthorName = post.Author.Name;
        CreatedAt = post.CreatedAt;
        Tags = post.MapTags(static x => x.Value).ToArray();
        RepliesQuantity = post.RepliesQuantity;
        ReplyToPostId = post.ReplyTo?.Id;
        Version = post.Version;
    }
    
    public Post MapToPost()
    {
        ReplyToPost? replyTo = ReplyToPostId.HasValue ? new ReplyToPost(ReplyToPostId.Value) : null;
        var tags = Tags?.Select(x => new Tag(x)).ToArray();
        
        return new Post(Id, Content, replyTo, CreatedAt, new Author(AuthorId, AuthorName), tags,
            Likes, RepliesQuantity, Version);
    }
}