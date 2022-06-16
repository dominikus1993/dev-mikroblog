using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Events;

public class PostCreated : IMessage
{
    public required Guid MessageId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid PostId { get; init; }
    public required string Content { get; init; }
    public required Guid AuthorId { get; set; }
    public static string Name => nameof(PostCreated);
}