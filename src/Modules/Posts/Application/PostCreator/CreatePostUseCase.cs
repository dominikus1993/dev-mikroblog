using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.BuildingBlocks.Infrastructure.Time;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
using DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

namespace DevMikroblog.Modules.Posts.Application.PostCreator;

public record CreatePostCommand(Author Author, string Content, ReplyToPost? ReplyTo);

public sealed class CreatePostUseCase
{
    private readonly IPostWriter _postWriter;
    private readonly IMessagePublisher<PostCreated> _messagePublisher;
    private readonly IPostTagParser _postTagParser;
    private readonly ISystemClock _systemClock;

    public CreatePostUseCase(IPostWriter postWriter, IMessagePublisher<PostCreated> messagePublisher, IPostTagParser postTagParser, ISystemClock systemClock)
    {
        _postWriter = postWriter;
        _messagePublisher = messagePublisher;
        _postTagParser = postTagParser;
        _systemClock = systemClock;
    }

    public async Task Execute(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var tags = _postTagParser.ParseTagsFromPostContent(command.Content);
        var post = Post.CreateNew(command.Content, _systemClock.Now(), command.Author, tags, command.ReplyTo);

        await _postWriter.Add(post, cancellationToken);

        await _messagePublisher.Publish(new PostCreated()
        {
            Content = command.Content,
            AuthorId = command.Author.Id.Value,
            CreatedAt = post.CreatedAt,
            MessageId = Guid.NewGuid(),
            PostId = post.Id.Value,
            ReplyToPost = command.ReplyTo?.Id.Value
        }, cancellationToken);
    }
}