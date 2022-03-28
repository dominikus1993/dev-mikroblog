using System.Text.Json;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    private static readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbtMqSubscriptionConfig<T> _config;

    private readonly IModel _channel;

    public RabbitMqSubscriber(IServiceProvider serviceProvider, RabbtMqSubscriptionConfig<T> config, IModel channel)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        _channel = channel;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.ExchangeDeclare(exchange: _config.Exchange, type: ExchangeType.Topic);
        _channel.QueueDeclare(queue: _config.Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: _config.Queue, exchange: _config.Exchange, routingKey: _config.Topic);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;
        _channel.BasicConsume(queue: _config.Queue, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
    {
        var message = JsonSerializer.Deserialize<T>(ea.Body, _options);
        var messageHandler = _serviceProvider.GetService<IMessageHandler<T>>();
        return messageHandler!.Handle(message);
    }
}