using DevMikroblog.Modules.Posts.Infrastructure.Configuration;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

using Microsoft.EntityFrameworkCore;

using Testcontainers.PostgreSql;

using Xunit;

namespace Posts.FunctionalTests.Fixtures;

public sealed class PostgresSqlSqlFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgreSql { get; }
    internal PostDbContext Context { get; private set; }

    public PostgresSqlSqlFixture()
    {
        this.PostgreSql = new PostgreSqlBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await this.PostgreSql.StartAsync()
            .ConfigureAwait(false);

        var builder = new DbContextOptionsBuilder<PostDbContext>().UseNpgsql(optionsBuilder =>
        {
            optionsBuilder.EnableRetryOnFailure(5);
            optionsBuilder.CommandTimeout(500);
            optionsBuilder.MigrationsAssembly(typeof(PostDbContext).Assembly.FullName);
        }).UseSnakeCaseNamingConvention();
        Context = new PostDbContext(builder.Options);
    }

    public async Task DisposeAsync()
    {
        await this.PostgreSql.DisposeAsync()
            .ConfigureAwait(false);

        await this.Context.DisposeAsync();
    }
}
