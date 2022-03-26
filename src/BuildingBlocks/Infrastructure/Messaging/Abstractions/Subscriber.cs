using LanguageExt;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessageHandler<T> where T : notnull, IMessage
{
    Task<Unit> Handle(T message, CancellationToken cancellationToken = default);
}