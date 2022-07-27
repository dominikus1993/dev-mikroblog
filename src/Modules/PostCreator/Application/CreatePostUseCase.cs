using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

using PostCreator.Application.Events;
using PostCreator.Application.Parsers;
using PostCreator.Domain.Model;
using PostCreator.Domain.Repositories;

namespace PostCreator.Application;

public record CreatePostCommand(Author Author, string Content, ReplyToPost? ReplyTo);

internal sealed class CreatePostUseCase
{
    private readonly IPostWriter _postWriter;
    private readonly IMessagePublisher<PostCreated> _messagePublisher;
    private readonly IPostTagParser _postTagParser;

    public CreatePostUseCase(IPostWriter postWriter, IMessagePublisher<PostCreated> messagePublisher, IPostTagParser postTagParser)
    {
        _postWriter = postWriter;
        _messagePublisher = messagePublisher;
        _postTagParser = postTagParser;
    }

    public async Task Execute(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var tags = _postTagParser.ParseTagsFromPostContent(command.Content);
        var post = Post.CreateNew(command.Content, command.Author, tags, command.ReplyTo);

        await _postWriter.Save(post, cancellationToken);

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