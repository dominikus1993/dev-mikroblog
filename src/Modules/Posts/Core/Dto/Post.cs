using DevMikroblog.Modules.Posts.Core.Model;

namespace DevMikroblog.Modules.Posts.Core.Dto;

public class PostDto
{
    private PostDto(Post post)
    {
        
    }

    public static PostDto FromPost(Post post) => new(post);

}