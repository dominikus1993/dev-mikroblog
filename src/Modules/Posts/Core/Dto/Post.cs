using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Core.Dto;

public sealed class AuthorDto
{
    public string? AuthorName { get; init; }
    public Guid AuthorId { get; init; }
}

public sealed class PostDto
{
    public Guid PostId { get; init; }
    public AuthorDto Author { get; init; } = null!;
    public string Content { get; init; }
    public List<string>? Tags { get; init; }
    public int Likes { get; init; }
    
    public int RepliesQuantity { get; init; }
    
    
    private PostDto(Post post)
    {
        PostId = post.Id.Value;
        Content = post.Content;
        Author = new AuthorDto() { AuthorId = post.Author.Id.Value, AuthorName = post.Author.Name };
        Likes = post.Likes;
        Tags = post.Tags.Map(tags => tags.Select(tag => tag.Value).ToList()).IfNoneUnsafe(() => null);
        RepliesQuantity = post.RepliesQuantity;
    }

    public static PostDto FromPost(Post post) => new(post);
}