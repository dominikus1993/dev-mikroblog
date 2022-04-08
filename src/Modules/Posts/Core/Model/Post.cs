using StronglyTypedIds;

namespace DevMikroblog.Modules.Posts.Core.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct PostId{}

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct AuthorId {}

public record Author(AuthorId Id, string? Name);

public record Post(PostId Id, string Content, IList<Post> Replies, DateTime CreatedAt, Author Author, int Likes);