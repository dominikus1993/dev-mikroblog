using System.Collections.Generic;
namespace DevMikroblog.Modules.Posts.Core.Model;

[StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
public partial struct PostId{}
public record Post(PostId Id, string Content, IList<Post> Replies, DateTime CreatedAt, int Linkes);