using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Serialization;

public interface IMessageSerializer<T> where T: IMessage
{
    T? Deserialize(in ReadOnlyMemory<byte> body);
    ReadOnlyMemory<byte> Serialize(T body);
}