using System.Runtime.CompilerServices;

using LanguageExt;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Model;

public readonly record struct PostId(Guid Value)
{
    public static PostId New() => new PostId(Guid.NewGuid());
}

public readonly record struct AuthorId(Guid Value)
{
    public static AuthorId New() => new AuthorId(Guid.NewGuid());
}

public sealed record ReplyToPost(PostId Id);
public sealed class Post
{
    public Post(PostId Id, string Content, ReplyToPost? ReplyTo, DateTimeOffset CreatedAt, Author Author, string[]? Tags, uint Likes, uint RepliesQuantity, uint Version = 1)
    {
        this.Id = Id;
        this.Content = Content;
        this.ReplyTo = ReplyTo;
        this.CreatedAt = CreatedAt;
        this.Author = Author;
        this.Tags = Tags;
        this.Likes = Likes;
        this.RepliesQuantity = RepliesQuantity;
        this.Version = Version;
    }

    public Post()
    {
        
    }

    public void IncrementRepliesQuantity()
    {
        Likes += 1;
    }
    

    public static Post CreateNew(string content, DateTimeOffset dateTime, Author author, string[]? tags, ReplyToPost? replyTo = null)
    {
        var id = PostId.New();
        return new Post(id, content, replyTo, dateTime, author, tags, 0, 0, 1);
    }

    public IEnumerable<T> MapTags<T>(Func<string, T> mapF)
    {
        if (Tags is null or { Length: 0 })
        {
            yield break;
        }

        if (Tags is [var t])
        {
            yield return mapF(t);
            yield break;
        }

        foreach (string tag in Tags)
        {
            yield return mapF(tag);
        }
    }

    public void Delete(DateTimeOffset deleteAt)
    {
        IsDeleted = true;
        DeletedAt = deleteAt;
    }
    
    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public PostId Id { get; init; }
    public string Content { get; init; }
    public ReplyToPost? ReplyTo { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public Author Author { get; init; }
    public string[]? Tags { get; init; }
    public uint Likes { get; set; }
    public uint RepliesQuantity { get; init; }
    public uint Version { get; init; }
}