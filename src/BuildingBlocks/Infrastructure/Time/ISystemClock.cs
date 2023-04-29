namespace DevMikroblog.BuildingBlocks.Infrastructure.Time;

public interface ISystemClock
{
    DateTimeOffset Now();
}

public sealed class UtcSystemClock: ISystemClock
{
    public DateTimeOffset Now() => DateTimeOffset.UtcNow;
}