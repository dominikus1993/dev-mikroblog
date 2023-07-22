using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Events;

public sealed class PostCreated : IMessage
{
    public Guid MessageId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid PostId { get; set; }
    public Guid? ReplyToPost { get; set; }
    public string Content { get; set; } = null!;
    public Guid AuthorId { get; set; }
    
    public List<string>? Tags { get; set; }
}