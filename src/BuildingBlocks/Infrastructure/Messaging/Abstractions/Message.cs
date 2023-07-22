namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessage
{
    Guid MessageId { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    static abstract string Name { get; }
}