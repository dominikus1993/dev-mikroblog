using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

namespace PostCreator.Application.Events;

public class PostCreated : IMessage
{
    public Guid MessageId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid PostId { get; init; }
    public Guid? ReplyToPost { get; init; }
    public string Content { get; init; } = null!;
    public Guid AuthorId { get; init; }
    
    public List<string>? Tags { get; init; }
    public static string Name => nameof(PostCreated);
}