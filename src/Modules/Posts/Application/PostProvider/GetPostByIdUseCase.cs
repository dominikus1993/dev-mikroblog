using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Application.PostProvider;

public sealed class GetPostByIdUseCase
{
    private IPostsReader _postsReader;

    public GetPostByIdUseCase(IPostsReader postsReader)
    {
        _postsReader = postsReader;
    }

    public async Task<Option<PostDto>> Execute(PostId id, CancellationToken cancellationToken)
    {
        var result = await _postsReader.GetPostById(id, cancellationToken);
        return result.Map(x => PostDto.FromPost(x));
    }
}