namespace DevMikroblog.Modules.Posts.Core.Model;

public readonly record struct PostId(Guid Value);
public record Post(PostId Id, string Content);