using System.Collections.ObjectModel;
using System.Threading.Channels;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Configuration;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.OpenTelemetry;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Publisher;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Subscriber;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Trace;

using RabbitMQ.Client;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddRabbitMq(configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>());
    }
    
    public static IHealthChecksBuilder AddRabbitMq(this IHealthChecksBuilder services)
    {
        return services.AddRabbitMQ();
    }
    
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, RabbitMqConfiguration? configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(configuration.AmqpConnection, $"{nameof(configuration)}.{nameof(configuration.AmqpConnection)}");
        services.AddSingleton<IConnectionFactory>(new ConnectionFactory()
        {
            Uri = new Uri(configuration.AmqpConnection),
            DispatchConsumersAsync = true
        });
        services.AddSingleton<IConnection>(sp => sp.GetService<IConnectionFactory>()!.CreateConnection());
        services.AddScoped<IModel>(sp => sp.GetService<IConnection>()!.CreateModel());
        services.AddSingleton(sp => new RabbitMqPublishChannel(sp.GetService<IConnection>()!.CreateModel()));
        return services;
    }
    
    public static IServiceCollection AddPublisher<TMessage>(this IServiceCollection services, string exchangeName, string topic = "#") where TMessage : class, IMessage
    {
        var config = new RabbitMqPublisherConfig<TMessage> { Exchange = exchangeName, Topic = topic };
        services.AddSingleton(config);
        services.AddTransient<IMessagePublisher<TMessage>, RabbitMqMessagePublisher<TMessage>>();
        return services;
    }
    
    public static IServiceCollection AddSubscriber<TMessage, THandler>(this IServiceCollection services, string exchange, string topic = "#", string? queuename = null) where TMessage : notnull, IMessage where THandler: class, IMessageHandler<TMessage>
    {
        queuename ??= $"{exchange}.{typeof(TMessage).FullName}";
        var config = new RabbtMqSubscriptionConfig<TMessage> { Exchange = exchange, Topic = topic, Queue = queuename};
        services.AddSingleton(config);
        services.AddTransient<IMessageHandler<TMessage>, THandler>();
        services.AddHostedService<RabbitMqSubscriber<TMessage>>();
        return services;
    }

    public static TracerProviderBuilder AddRabbitMqTelemetry(this TracerProviderBuilder builder)
    {
        return builder.AddSource(RabbitMqOpenTelemetry.RabbitMqOpenTelemetrySourceName);
    }
}