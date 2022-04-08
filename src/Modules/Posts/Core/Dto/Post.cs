using DevMikroblog.Modules.Posts.Core.Model;

namespace DevMikroblog.Modules.Posts.Core.Dto;

public sealed class PostDto
{
    public Guid PostId { get; init; }

    public string Content { get; set; }
    
    private PostDto(Post post)
    {
        PostId = post.Id.Value;
        Content = post.Content;
    }

    public static PostDto FromPost(Post post) => new(post);

}