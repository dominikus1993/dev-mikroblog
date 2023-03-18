using DevMikroblog.Modules.Posts.Infrastructure.Configuration;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

using Testcontainers.PostgreSql;

using Xunit;

namespace Posts.FunctionalTests.Fixtures;

public sealed class PostgresSqlSqlFixture : IAsyncLifetime, IDisposable
{
    public PostgreSqlContainer PostgreSql { get; }
    internal DocumentStore Store { get; private set; }

    public PostgresSqlSqlFixture()
    {
        this.PostgreSql = new PostgreSqlBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await this.PostgreSql.StartAsync()
            .ConfigureAwait(false);
        Store = DocumentStore.For(MartenDocumentStoreConfig.Configure(PostgreSql.ConnectionString, true));
    }

    public async Task DisposeAsync()
    {
        await this.PostgreSql.DisposeAsync()
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        Store.Dispose();
    }
}