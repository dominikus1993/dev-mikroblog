namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Publisher;

using System.Threading;
using System.Threading.Tasks;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

using LanguageExt;

public class RabbitmMqMessagePublishe<T> : IMessagePublisher<T> where T : notnull, IMessage
{
    public Task<Unit> Publish(T message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}