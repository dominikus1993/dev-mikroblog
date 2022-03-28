using System.ComponentModel;
namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Publisher;

using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Logging;

using LanguageExt;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

public class RabbtMqPublisheConfig<T> where T : notnull, IMessage
{
    public string Exchange { get; init; } = null!;
    public string Topic { get; init; } = "#";
}

public readonly record struct RabbitMqMessage(ReadOnlyMemory<byte> Body, string Exchange, string Topic);

internal class RabbitmMqMessagePublisher<T> : IMessagePublisher<T> where T : notnull, IMessage
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly RabbtMqPublisheConfig<T> _config;
    private readonly ChannelWriter<RabbitMqMessage> _stream;

    public RabbitmMqMessagePublisher(RabbtMqPublisheConfig<T> config, Channel<RabbitMqMessage> stream)
    {
        _config = config;
        _stream = stream.Writer;
    }

    public async ValueTask<Unit> Publish(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var json = JsonSerializer.SerializeToUtf8Bytes(message, _options);
        await _stream.WriteAsync(new RabbitMqMessage(json, _config.Exchange, _config.Topic), cancellationToken);
        return Unit.Default;
    }
}

internal class RabbitMqPublisher : BackgroundService
{
    private readonly ChannelReader<RabbitMqMessage> _messageStream;
    private readonly IModel _model;
    private readonly ILogger<RabbitMqPublisher> _logger;

    private readonly Dictionary<string, object> _defaultHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "X-Message-Type", "DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions.RabbitmMqMessage" },
    };

    public RabbitMqPublisher(Channel<RabbitMqMessage> stream, IModel model, ILogger<RabbitMqPublisher> logger)
    {
        _messageStream = stream.Reader;
        _model = model;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        await foreach (var message in _messageStream.ReadAllAsync(stoppingToken))
        {
            _model.ExchangeDeclare(exchange: message.Exchange, type: ExchangeType.Topic);
            var props = _model.CreateBasicProperties();
            props.Headers = _defaultHeaders;
            _model.BasicPublish(exchange: message.Exchange, routingKey: message.Topic, basicProperties: props, body: message.Body);
            _logger.LogMessagePublished(message.Exchange, message.Topic);
        }
    }
}