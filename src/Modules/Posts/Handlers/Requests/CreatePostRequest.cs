namespace DevMikroblog.Modules.Posts.Handlers.Requests;

public sealed class CreatePostRequest
{
    public string Content { get; init; }
    public Guid? ReplyToPostId { get; set; }
}