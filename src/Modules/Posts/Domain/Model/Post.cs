using System.Runtime.CompilerServices;

using LanguageExt;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Model;

public readonly record struct PostId(Guid Value)
{
    public static PostId New() => new PostId(Guid.NewGuid());
}

public readonly record struct AuthorId(Guid Value);
public readonly record struct Tag(string Value);
public sealed record Author(AuthorId Id, string? Name);
public readonly record struct ReplyToPost(PostId Id);
public sealed record Post(PostId Id, string Content, ReplyToPost? ReplyTo, DateTime CreatedAt, Author Author, IReadOnlyList<Tag>? Tags, int Likes, int RepliesQuantity)
{
    public Post IncrementRepliesQuantity()
    {
        return this with { RepliesQuantity = RepliesQuantity + 1 };
    }
    

    public static Post CreateNew(string content, Author author, IReadOnlyList<Tag>? tags, ReplyToPost? replyTo = null)
    {
        var id = PostId.New();
        return new Post(id, content, replyTo, DateTime.UtcNow, author, tags, 0, 0);
    }

    public IEnumerable<T> MapTags<T>(Func<Tag, T> mapF)
    {
        if (Tags is null or { Count: 0 })
        {
            yield break;
        }

        if (Tags is [var t])
        {
            yield return mapF(t);
            yield break;
        }

        foreach (Tag tag in Tags)
        {
            yield return mapF(tag);
        }
    }
}