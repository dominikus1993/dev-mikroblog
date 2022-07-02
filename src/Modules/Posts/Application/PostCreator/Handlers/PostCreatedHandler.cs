using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

using Microsoft.Extensions.Logging;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Handlers;

public class PostCreatedHandler : IMessageHandler<PostCreated>
{
    private IPostsReader _postsReader;
    private IPostWriter _postWriter;
    private ILogger<PostCreatedHandler> _logger;

    public PostCreatedHandler(IPostsReader postsReader, IPostWriter postWriter, ILogger<PostCreatedHandler> logger)
    {
        _postsReader = postsReader;
        _postWriter = postWriter;
        _logger = logger;
    }

    public async Task<Unit> Handle(PostCreated message, CancellationToken cancellationToken = default)
    {
        if (message.ReplyToPost.HasValue)
        {
            var replyPostId = new PostId(message.ReplyToPost.Value);
            var postOpt = await _postsReader.GetPostById(replyPostId, cancellationToken);
            await postOpt.IfSomeAsync(async post =>
            {
                var res = post.IncrementRepliesQuantity();
                await _postWriter.Save(res, cancellationToken);
            });
        }
        return Unit.Default;
    }
}