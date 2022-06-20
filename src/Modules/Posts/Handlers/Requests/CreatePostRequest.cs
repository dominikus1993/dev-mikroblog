namespace DevMikroblog.Modules.Posts.Handlers.Requests;

public class CreatePostRequest
{
    public string Content { get; init; }
    public Guid ReplyToPostId { get; set; }
}