using System.Text.Json;
using System.Text.Json.Serialization;

using AutoFixture.Xunit2;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Serialization;

namespace Infrastructure.Tests.Messaging.Serialization;

public sealed class TestMsg : IMessage
{
    public string Msg { get; set; }
    public Guid MessageId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public static string Name { get; } = typeof(TestMsg).FullName!;
}

[JsonSerializable(typeof(TestMsg))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public sealed partial class TestJsonContext : JsonSerializerContext
{
    
}

public sealed class SystemTextMessageSerializerTests
{
    private IMessageSerializer<TestMsg> _messageSerializer;

    public SystemTextMessageSerializerTests()
    {
        _messageSerializer = new SystemTextMessageSerializer<TestMsg>(TestJsonContext.Default.Options);
    }

    [Theory]
    [AutoData]
    public void TestMessageSerialization(TestMsg msg)
    {
        var json = _messageSerializer.Serialize(msg);

        var subject = _messageSerializer.Deserialize(ref json);

        Assert.NotNull(subject);
        Assert.Equivalent(msg, subject);
    }
}