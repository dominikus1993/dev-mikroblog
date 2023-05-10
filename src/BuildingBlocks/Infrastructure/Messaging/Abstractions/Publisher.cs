using LanguageExt;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessagePublisher<in T> where T : class, IMessage
{
    ValueTask<Unit> Publish(T message, CancellationToken cancellationToken = default);
}