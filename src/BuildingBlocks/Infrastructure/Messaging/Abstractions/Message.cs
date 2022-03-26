using System;
namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;

public interface IMessage
{
    Guid MessageId { get; init; }
    DateTime CreatedAt { get; init; }
}