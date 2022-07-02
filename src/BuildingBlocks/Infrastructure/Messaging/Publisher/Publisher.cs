using System.ComponentModel;
using System.Diagnostics;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.OpenTelemetry;

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

internal class RabbitMqPublishChannel : IDisposable
{
    public IModel Model { get; }

    public RabbitMqPublishChannel(IModel model)
    {
        Model = model;
    }

    public void Dispose()
    {
        if (!Model.IsClosed)
        {
            Model.Dispose();
        }
    }
}
internal class RabbitMqMessagePublisher<T> : IMessagePublisher<T> where T : notnull, IMessage
{
    private static readonly JsonSerializerOptions
        _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly RabbitMqPublisherConfig<T> _config;
    private readonly RabbitMqPublishChannel _channel;
    private readonly ILogger<RabbitMqMessagePublisher<T>> _logger;
    private static readonly Dictionary<string, object> DefaultHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "X-Message-Type", typeof(T).FullName },
        { "X-Message-Name", T.Name }
    };

    public RabbitMqMessagePublisher(RabbitMqPublisherConfig<T> config, RabbitMqPublishChannel channel,
        ILogger<RabbitMqMessagePublisher<T>> logger)
    {
        _config = config;
        _logger = logger;
        _channel = channel;
    }

    public ValueTask<Unit> Publish(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        _logger.LogPublishRabbitMqMessage(T.Name, _config.Exchange, _config.Topic);
        var json = JsonSerializer.SerializeToUtf8Bytes(message, _options);
        using var activity = RabbitMqOpenTelemetry.RabbitMqSource.StartActivity("rabbitmq", ActivityKind.Producer);
        _channel.Model.ExchangeDeclare(exchange: _config.Exchange, type: ExchangeType.Topic);
        var props = _channel.Model.CreateBasicProperties();
        InjectIntoHeader(props);
        if (activity is not null)
        {
                
            activity.SetTag("messaging.rabbitmq.routing_key", _config.Topic);
            activity.SetTag("messaging.destination", _config.Exchange);
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.destination_kind", "topic");
            activity.SetTag("messaging.protocol", "AMQP");
            activity.SetTag("messaging.protocol_version", "0.9.1");
            activity.SetTag("messaging.message_name", T.Name);
            RabbitMqOpenTelemetry.AddActivityToHeader(activity, props);
        }
        _channel.Model.BasicPublish(exchange: _config.Exchange, routingKey: _config.Topic, basicProperties: props,
            body: json);
        _logger.LogMessagePublished(_config.Exchange, _config.Topic);
        return new ValueTask<Unit>(Unit.Default);
    }

    private void InjectIntoHeader(IBasicProperties properties)
    {
        properties.Headers ??= new Dictionary<string, object>();
        foreach (var header in DefaultHeaders)
        {
            properties.Headers.Add(header.Key, header.Value);
        }
    }
}