using System.Runtime.CompilerServices;

using StronglyTypedIds;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct PostId{}

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct AuthorId {}

public record Author(AuthorId Id, string? Name);
public readonly record struct ReplyToPost(PostId Id);
public record Post(PostId Id, string Content, ReplyToPost? ReplyTo, DateTime CreatedAt, Author Author, int Likes)
{
    public static Post CreateNew(string content, Author author, ReplyToPost? replyTo = null)
    {
        var id = PostId.New();
        return new Post(id, content, replyTo, DateTime.UtcNow, author, 0);
    }
}