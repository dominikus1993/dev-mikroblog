using System.Collections.ObjectModel;
using System.Threading.Channels;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Configuration;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Publisher;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Trace;

using RabbitMQ.Client;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;

public static class ServicesCollectionExtensions
{
    internal const string RabbitMqOpenTelemetrySourceName = $"{nameof(DevMikroblog)}.Rabbitmq";
    
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
            Uri = new Uri(configuration.AmqpConnection)
        });
        services.AddSingleton<IConnection>(sp => sp.GetService<IConnectionFactory>()!.CreateConnection());
        services.AddScoped<IModel>(sp => sp.GetService<IConnection>()!.CreateModel());
        services.AddSingleton(sp => new RabbitMqPublishChannel(sp.GetService<IConnection>()!.CreateModel()));
        return services;
    }
    
    public static IServiceCollection AddPublisher<TMessage>(this IServiceCollection services, string exchangeName, string topic = "#") where TMessage : notnull, IMessage
    {
        var config = new RabbitMqPublisherConfig<TMessage> { Exchange = exchangeName, Topic = topic };
        services.AddSingleton(config);
        services.AddTransient<IMessagePublisher<TMessage>, RabbitMqMessagePublisher<TMessage>>();
        return services;
    }
    
    public static IServiceCollection AddSubscriber(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static TracerProviderBuilder AddRabbitMqTelemetry(this TracerProviderBuilder builder)
    {
        return builder.AddSource(RabbitMqOpenTelemetrySourceName);
    }
}