using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using LanguageExt;

using Microsoft.Extensions.Logging;

using PostCreator.Domain.Model;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Handlers;

public class PostCreatedHandler : IMessageHandler<PostCreated>
{
    private IPostModifier _modifier;

    public PostCreatedHandler(IPostModifier modifier)
    {
        _modifier = modifier;
    }

    public async Task<Unit> Handle(PostCreated message, CancellationToken cancellationToken = default)
    {
        if (message.ReplyToPost.HasValue)
        {
            var postId = new PostId(message.ReplyToPost.Value);
            await _modifier.Modify(postId, post => post.IncrementRepliesQuantity(), cancellationToken);
        }
        return Unit.Default;
    }
}