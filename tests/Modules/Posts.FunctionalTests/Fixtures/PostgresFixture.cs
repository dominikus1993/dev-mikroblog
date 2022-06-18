using DevMikroblog.Modules.Posts.Infrastructure.Configuration;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Marten;

using Weasel.Core;

using Xunit;

namespace Posts.FunctionalTests.Fixtures;

public sealed class PostgresSqlSqlFixture : IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration configuration = new PostgreSqlTestcontainerConfiguration("postgres:14-alpine") { Database = "posts", Username = "postgres", Password = "postgres" };

    public PostgreSqlTestcontainer PostgreSql { get; }
    internal DocumentStore Store { get; private set; }

    public PostgresSqlSqlFixture()
    {
        this.PostgreSql = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(this.configuration)
            .Build();
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
        this.configuration.Dispose();
    }
}