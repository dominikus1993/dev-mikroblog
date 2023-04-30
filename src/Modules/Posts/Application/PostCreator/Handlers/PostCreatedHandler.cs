using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

using Microsoft.Extensions.Logging;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Handlers;

public class PostCreatedHandler : IMessageHandler<PostCreated>
{
    private readonly IPostsReader _postsReader;
    private readonly IPostWriter _postWriter;

    public PostCreatedHandler(IPostsReader postsReader, IPostWriter postWriter)
    {
        _postsReader = postsReader;
        _postWriter = postWriter;
    }

    public async Task<Unit> Handle(PostCreated message, CancellationToken cancellationToken = default)
    {
        if (message.ReplyToPost.HasValue)
        {
            var postId = new PostId(message.ReplyToPost.Value);
            var post = await _postsReader.GetPostById(postId, cancellationToken);
            if (post is null)
            {
                return Unit.Default;
            }

            var newPost = post.IncrementRepliesQuantity();
            await _postWriter.Update(newPost, cancellationToken);
        }
        return Unit.Default;
    }
}