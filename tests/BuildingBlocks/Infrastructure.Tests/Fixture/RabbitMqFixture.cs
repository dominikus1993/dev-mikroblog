using System.Diagnostics.CodeAnalysis;

using RabbitMQ.Client;

using Testcontainers.RabbitMq;

namespace Infrastructure.Tests.Fixture;

public sealed class RabbitMqFixture : IAsyncLifetime, IDisposable
{
   
    public RabbitMqFixture()
    {
        RabbitMq = new RabbitMqBuilder().Build();

    }
    
    public RabbitMqContainer RabbitMq { get; private set; }
    public ConnectionFactory ConnectionFactory { get; private set; }
    public IConnection Connection { get; private set; } = null!;

    [MemberNotNull(nameof(Connection))]
    public async Task InitializeAsync()
    {
        await RabbitMq.StartAsync();
        ConnectionFactory = new ConnectionFactory() { Uri = new Uri(RabbitMq.GetConnectionString()) };
    }

    public async Task DisposeAsync()
    {
        await RabbitMq.DisposeAsync();
    }

    public void Dispose()
    {
        Connection.Dispose();
    } 
}

[CollectionDefinition(nameof(RabbitMqFixtureCollectionTest))]
public class RabbitMqFixtureCollectionTest : ICollectionFixture<RabbitMqFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}