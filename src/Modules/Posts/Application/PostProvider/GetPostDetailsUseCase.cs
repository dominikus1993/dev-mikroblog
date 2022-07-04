using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Application.PostProvider;

public record PostDetailsDto(PostDto? ReplyTo, PostDto Post, IReadOnlyList<PostDto>? Replies)
{
    internal static PostDetailsDto FromPostDetails(PostDetails details)
    {
        var replies = details.Replies.Map<IReadOnlyList<PostDto>>(posts => posts.Select(x => PostDto.FromPost(x)).ToList()).IfNoneUnsafe(() => null);
        var replyTo = details.ReplyTo.Map(post => PostDto.FromPost(post)).IfNoneUnsafe(() => null);
        var post = PostDto.FromPost(details.Post);

        return new PostDetailsDto(replyTo, post, replies);
    }
}

public sealed class GetPostDetailsUseCase
{
    private readonly IPostsReader _postsReader;

    public GetPostDetailsUseCase(IPostsReader postsReader)
    {
        _postsReader = postsReader;
    }

    public async Task<Option<PostDetailsDto>> Execute(PostId id, CancellationToken cancellationToken)
    {
        var result = await _postsReader.GetPostDetails(id, cancellationToken);
        return result.Map(x => PostDetailsDto.FromPostDetails(x));
    }
}