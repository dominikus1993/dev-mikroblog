using System.Text.Json;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Serialization;

public sealed class SystemTextMessageSerializer<T> : IMessageSerializer<T> where T: IMessage
{
    private JsonSerializerOptions _serializerOptions;

    public SystemTextMessageSerializer(JsonSerializerOptions serializerOptions)
    {
        _serializerOptions = serializerOptions;
    }

    public T? Deserialize(ref ReadOnlyMemory<byte> body)
    {
        ReadOnlySpan<byte> json = body.Span;
        var reader = new Utf8JsonReader(json);
        return JsonSerializer.Deserialize<T>(ref reader, _serializerOptions);
    }

    public ReadOnlyMemory<byte> Serialize(T body)
    {
        return JsonSerializer.SerializeToUtf8Bytes(body, _serializerOptions);
    }
}