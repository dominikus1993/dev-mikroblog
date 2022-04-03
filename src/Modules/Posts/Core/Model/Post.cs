using StronglyTypedIds;

namespace DevMikroblog.Modules.Posts.Core.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct PostId{}
public record Post(PostId Id, string Content, IList<Post> Replies, DateTime CreatedAt, int Likes);