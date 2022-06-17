using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Infrastructure.Model;

internal class EfPost
{
    public Guid Id { get; init; }
    public string? Content { get; init; }
    public int Likes { get; set; }

    public Guid AuthorId { get; init; }
    public string AuthorName { get; set; }
    public DateTime CreatedAt { get; init; }
    
    public EfPost? ReplyTo { get; set; }
    public Guid? ReplyToPostId { get; init; }
    public void IncrementLikes()
    {
        Likes += 1;
    }
    
    public Post MapToPost()
    {
        ReplyToPost? replyTo = ReplyToPostId.HasValue ? new ReplyToPost(new PostId(ReplyToPostId.Value)) : null;

        return new Post(new PostId(Id), Content, replyTo, CreatedAt, new Author(new AuthorId(AuthorId), AuthorName),
            Likes);
    }
}