using LanguageExt;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessagePublisher<T> where T : class, IMessage
{
    ValueTask<Unit> Publish(T message, CancellationToken cancellationToken = default);
}