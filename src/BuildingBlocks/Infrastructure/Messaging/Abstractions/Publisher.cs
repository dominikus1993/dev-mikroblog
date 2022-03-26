using LanguageExt;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessagePublisher<T> where T : notnull, IMessage
{
    Task<Unit> Publish(T message, CancellationToken cancellationToken = default);
}