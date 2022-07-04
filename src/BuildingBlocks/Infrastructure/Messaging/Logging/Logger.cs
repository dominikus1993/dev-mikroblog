using Microsoft.Extensions.Logging;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Logging;

internal static partial class RabbitMqLogger
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "RabbitMQ Message Published on Exchange {Exchange} with Topic {Topic}")]
    public static partial void LogMessagePublished(this ILogger logger, string exchange, string topic);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "RabbitMQ Message from Exchange {Exchange} with Topic {Topic} is null or can't be deserialized")]
    public static partial void LogRabbitmqMessageIsNull(this ILogger logger, string exchange, string topic);
    
    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Publish RabbitMq message of {Name} to {Exchange} and {Topic}")]
    public static partial void LogPublishRabbitMqMessage(this ILogger logger, string name, string exchange, string topic);
    
    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Received RabbitMq message of {Name} to {Exchange}/{Queue} and {Topic}")]
    public static partial void LogReceivedRabbitMqMessage(this ILogger logger, string name, string exchange, string queue, string topic);
    
    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Error when processing rabbitmq message of {Name} to {Exchange}/{Queue} and {Topic}")]
    public static partial void LogProcessingMessageError(this ILogger logger, Exception exc, string name, string exchange, string queue, string topic);
    
    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "Start Rabbitmq consumer for Message {MessageName} on Exchange {Exchange} with Topic {Topic} and Queue {Queue}")]
    public static partial void LogConsumerStart(this ILogger logger, string messageName, string exchange, string topic, string queue);
}