using Microsoft.Extensions.Logging;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Logging;

internal static partial class RabbitMqLogger
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "RabbitMQ Message Publishe on Exchange {Exchange} with Topic {Topic}")]
    public static partial void LogMessagePublished(this ILogger logger, string exchange, string topic);
}