namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessage
{
    Guid MessageId { get; init; }
    DateTimeOffset CreatedAt { get; init; }
    static abstract string Name { get; }
}