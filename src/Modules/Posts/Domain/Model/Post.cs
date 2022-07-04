using System.Runtime.CompilerServices;

using LanguageExt;

using StronglyTypedIds;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct PostId{}

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct AuthorId {}
public readonly record struct Tag(string Value);
public record Author(AuthorId Id, string? Name);
public readonly record struct ReplyToPost(PostId Id);
public record Post(PostId Id, string Content, ReplyToPost? ReplyTo, DateTime CreatedAt, Author Author, Option<IReadOnlyList<Tag>> Tags, int Likes, int RepliesQuantity)
{
    public Post IncrementRepliesQuantity()
    {
        return this with { RepliesQuantity = RepliesQuantity + 1 };
    }
    
    public static Post CreateNew(string content, Author author, Option<IReadOnlyList<Tag>> tags, ReplyToPost? replyTo = null)
    {
        var id = PostId.New();
        return new Post(id, content, replyTo, DateTime.UtcNow, author, tags, 0, 0);
    }
}