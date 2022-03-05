using System.Collections.Generic;
namespace DevMikroblog.Modules.Posts.Core.Model;

public readonly record struct PostId(Guid Value);
public record Post(PostId Id, string Content, IList<Post> Replies, DateTime CreatedAt, int Linkes);