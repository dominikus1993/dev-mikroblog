using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Core.Dto;

public sealed class AuthorDto
{
    public string? AuthorName { get; init; }
    public Guid AuthorId { get; init; }

    public AuthorDto(Author author)
    {
        AuthorId = author.Id.Value;
        AuthorName = author.Name;
    }
}

public sealed class PostDto
{
    public Guid PostId { get; init; }
    public AuthorDto Author { get; init; }
    public string Content { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; }
    public int Likes { get; init; }
    
    public int RepliesQuantity { get; init; }
    
    
    private PostDto(Post post)
    {
        PostId = post.Id.Value;
        Content = post.Content;
        Author = new AuthorDto(post.Author);
        Likes = post.Likes;
        Tags = post.Tags?.Select(tag => tag.Value).ToArray();
        RepliesQuantity = post.RepliesQuantity;
    }

    public static PostDto FromPost(Post post) => new(post);
}