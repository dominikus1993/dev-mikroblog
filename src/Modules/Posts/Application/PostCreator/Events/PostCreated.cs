using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Events;

public class PostCreated : IMessage
{
    public Guid MessageId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid PostId { get; init; }
    public Guid? ReplyToPost { get; init; }
    public string Content { get; init; } = null!;
    public Guid AuthorId { get; set; }
    public static string Name => nameof(PostCreated);
}