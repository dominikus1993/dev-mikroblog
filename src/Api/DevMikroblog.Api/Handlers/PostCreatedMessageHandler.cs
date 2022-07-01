using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;

using LanguageExt;

namespace DevMikroblog.Api.Handlers;

public class PostCreatedMessageHandler : IMessageHandler<PostCreated>
{
    private ILogger<PostCreatedMessageHandler> _logger;

    public PostCreatedMessageHandler(ILogger<PostCreatedMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task<Unit> Handle(PostCreated message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("XDDD 21 {Content}", message.Content);
        return Task.FromResult(Unit.Default);
    }
}