using StronglyTypedIds;

namespace DevMikroblog.Modules.Posts.Domain.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct PostId{}

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct AuthorId {}

public record Author(AuthorId Id, string? Name);

public record Post(PostId Id, string Content, IList<Post> Replies, DateTime CreatedAt, Author Author, int Likes)
{
    public static Post CreateNew(string content, Author author)
    {
        var id = PostId.New();
        return new Post(id, content, new List<Post>(0), DateTime.UtcNow, author, 0);
    }
}