using Microsoft.Extensions.Logging;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Logging;

internal static partial class RabbitMqLogger
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "RabbitMQ Message Publishe on Exchange {Exchange} with Topic {Topic}")]
    public static partial void LogMessagePublished(this ILogger logger, string exchange, string topic);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "RabbitMQ Message from Exchange {Exchange} with Topic {Topic} is null or can't be deserialized")]
    public static partial void LogRabbitmqMessageIsNull(this ILogger logger, string exchange, string topic);
    
    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Publish RabbitMq message of {Name} to {Exchange} and {Topic}")]
    public static partial void LogPublishRabbitMqMessage(this ILogger logger, string exchange, string topic, string messageName);
}