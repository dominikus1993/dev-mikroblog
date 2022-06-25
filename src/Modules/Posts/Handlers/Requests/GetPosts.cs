namespace DevMikroblog.Modules.Posts.Handlers.Requests;

public class GetPostsRequest
{
    public string? AuthorId { get; set; }
    public string? Tag { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}