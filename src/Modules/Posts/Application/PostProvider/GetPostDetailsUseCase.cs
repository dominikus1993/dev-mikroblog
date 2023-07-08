using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Application.PostProvider;

public record PostDetailsDto(PostDto? ReplyTo, PostDto? Post)
{
    internal static PostDetailsDto FromPostDetails(PostDetails details)
    {
        var replyTo = details.ReplyTo is null ? null : PostDto.FromPost(details.ReplyTo);
        var post = PostDto.FromPost(details.Post);

        return new PostDetailsDto(replyTo, post);
    }
}

public sealed class GetPostDetailsUseCase
{
    private readonly IPostsReader _postsReader;

    public GetPostDetailsUseCase(IPostsReader postsReader)
    {
        _postsReader = postsReader;
    }

    public async Task<PostDetailsDto?> Execute(PostId id, CancellationToken cancellationToken)
    {
        var result = await _postsReader.GetPostDetails(id, cancellationToken);
        if (result is null)
        {
            return null;
        }

        return PostDetailsDto.FromPostDetails(result);
    }
}