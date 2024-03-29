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
    public IReadOnlyCollection<string>? Tags { get; init; }
    public uint Likes { get; init; }
    public uint RepliesQuantity { get; init; }
    
    
    private PostDto(Post post)
    {
        PostId = post.Id.Value;
        Content = post.Content;
        Author = new AuthorDto(post.Author);
        Likes = post.Likes;
        Tags = post.Tags;
        RepliesQuantity = post.RepliesQuantity;
    }

    public static PostDto FromPost(Post post) => new(post);
}