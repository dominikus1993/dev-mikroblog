using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Logging;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.OpenTelemetry;

using LanguageExt.Pretty;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Trace;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Subscriber;

public class RabbtMqSubscriptionConfig<T> where T : notnull, IMessage
{
    public string Exchange { get; init; } = null!;
    public string Topic { get; init; } = "#";
    public string Queue { get; init; } = null!;
}

internal class RabbitMqSubscriber<T> : BackgroundService where T : notnull, IMessage
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbtMqSubscriptionConfig<T> _config;
    private readonly ILogger<RabbitMqSubscriber<T>> _logger;
    private readonly IModel _channel;

    public RabbitMqSubscriber(IServiceProvider serviceProvider, RabbtMqSubscriptionConfig<T> config, IConnection connection, ILogger<RabbitMqSubscriber<T>> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        _channel = connection.CreateModel();
        _channel.BasicQos(0, 1, false);
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogConsumerStart(T.Name, _config.Exchange, _config.Topic, _config.Queue);
        _channel.ExchangeDeclare(exchange: _config.Exchange, type: ExchangeType.Topic);
        _channel.QueueDeclare(queue: _config.Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: _config.Queue, exchange: _config.Exchange, routingKey: _config.Topic);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;
        _channel.BasicConsume(queue: _config.Queue, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
    {
        _logger.LogReceivedRabbitMqMessage(T.Name, _config.Exchange, _config.Queue, _config.Topic);
        using var activity = RabbitMqOpenTelemetry.RabbitMqSource.StartActivity(name: "rabbitmq.consumer",
            ActivityKind.Consumer,
            RabbitMqOpenTelemetry.GetHeaderFromProps(ea.BasicProperties).ActivityContext);
        if (activity is not null)
        {
            activity.SetTag("messaging.rabbitmq.routing_key", _config.Topic);
            activity.SetTag("messaging.exchange", _config.Exchange);
            activity.SetTag("messaging.destination", _config.Queue);
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.destination_kind", "queue");
            activity.SetTag("messaging.protocol", "AMQP");
            activity.SetTag("messaging.protocol_version", "0.9.1");
            activity.SetTag("messaging.message_name", T.Name);
        }
        var message = JsonSerializer.Deserialize<T>(ea.Body.Span, Options);
        if (message is null)
        {
            _logger.LogRabbitmqMessageIsNull(ea.Exchange, ea.RoutingKey);
            activity?.AddEvent(new ActivityEvent("message is null"));
            return;
        }
        try
        {
            var messageHandler = _serviceProvider.GetService<IMessageHandler<T>>()!;
            await messageHandler!.Handle(message);
        }
        catch (Exception e)
        {
            activity?.RecordException(e);
            _logger.LogProcessingMessageError(e, T.Name, _config.Exchange, _config.Queue, _config.Topic);
        }

    }

    public override void Dispose()
    {
        if (!_channel.IsClosed)
        {
            _channel.Close();
        }
        _channel.Dispose();
        base.Dispose();
    }
}