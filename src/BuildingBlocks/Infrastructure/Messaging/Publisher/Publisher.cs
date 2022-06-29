using System.ComponentModel;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

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

public class RabbitMqPublisherConfig<T> where T : notnull, IMessage
{
    public string Exchange { get; init; } = null!;
    public string Topic { get; init; } = "#";

    public string MessageName { get; } = T.Name;
}

public readonly record struct RabbitMqMessage(ReadOnlyMemory<byte> Body, string Exchange, string Topic,
    string MessageName);

internal class RabbitMqMessagePublisher<T> : IMessagePublisher<T> where T : notnull, IMessage
{
    private static readonly JsonSerializerOptions
        _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly RabbitMqPublisherConfig<T> _config;
    private readonly ChannelWriter<RabbitMqMessage> _stream;
    private readonly ILogger<RabbitMqMessagePublisher<T>> _logger;

    public RabbitMqMessagePublisher(RabbitMqPublisherConfig<T> config, Channel<RabbitMqMessage> stream,
        ILogger<RabbitMqMessagePublisher<T>> logger)
    {
        _config = config;
        _logger = logger;
        _stream = stream.Writer;
    }

    public async ValueTask<Unit> Publish(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        _logger.LogPublishRabbitMqMessage(T.Name, _config.Exchange, _config.Topic);
        var json = JsonSerializer.SerializeToUtf8Bytes(message, _options);
        await _stream.WriteAsync(new RabbitMqMessage(json, _config.Exchange, _config.Topic, T.Name), cancellationToken);
        return Unit.Default;
    }
}

internal class RabbitMqPublisher : BackgroundService
{
    internal const string RabbitMqOpenTelemetrySourceName = $"{nameof(RabbitMqOpenTelemetrySourceName)}.Source";
    private readonly ChannelReader<RabbitMqMessage> _messageStream;
    private readonly IModel _model;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private static ActivitySource source = new(RabbitMqOpenTelemetrySourceName, "v1.0.0");

    private readonly Dictionary<string, object> _defaultHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "X-Message-Type", "DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions.RabbitmMqMessage" },
    };

    public RabbitMqPublisher(ChannelReader<RabbitMqMessage> stream, IConnection model,
        ILogger<RabbitMqPublisher> logger)
    {
        _messageStream = stream;
        _model = model.CreateModel();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
    {
        await foreach (var message in _messageStream.ReadAllAsync(stoppingToken))
        {
            using var activity = source.StartActivity("rabbitmq", ActivityKind.Internal);
            _model.ExchangeDeclare(exchange: message.Exchange, type: ExchangeType.Topic);
            var props = _model.CreateBasicProperties();
            props.Headers = _defaultHeaders;
            props.Headers["X-Message-Name"] = message.MessageName;
            if (activity is not null)
            {
                
                activity.SetTag("messaging.rabbitmq.routing_key", message.Topic);
                activity.SetTag("messaging.destination", message.Exchange);
                activity.SetTag("messaging.system", "rabbitmq");
                activity.SetTag("messaging.destination_kind", "topic");
                activity.SetTag("messaging.protocol", "AMQP");
                activity.SetTag("messaging.protocol_version", "0.9.1");
                activity.SetTag("messaging.message_name", message.MessageName);
                AddActivityToHeader(activity, props);
            }
            _model.BasicPublish(exchange: message.Exchange, routingKey: message.Topic, basicProperties: props,
                body: message.Body);
            _logger.LogMessagePublished(message.Exchange, message.Topic);
        }
    }
    
    private static void AddActivityToHeader(Activity activity, IBasicProperties props)
    {
        var context = new PropagationContext(activity.Context, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(context, props,(properties, key, value) =>  InjectContextIntoHeader(properties, key, value));
    }

    private static void InjectContextIntoHeader(IBasicProperties props, string key, string value)
    {
        props.Headers ??= new Dictionary<string, object>();
        props.Headers[key] = value;
    }

    public override void Dispose()
    {
        _model.Dispose();
        base.Dispose();
    }
}