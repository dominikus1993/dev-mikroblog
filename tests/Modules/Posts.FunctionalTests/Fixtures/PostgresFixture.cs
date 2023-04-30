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
    private class TestDbContextFactory : IDbContextFactory<PostDbContext>
    {
        private readonly DbContextOptionsBuilder<PostDbContext> _dbContextOptionsBuilder;
        public TestDbContextFactory(string connection)
        {
            _dbContextOptionsBuilder = new DbContextOptionsBuilder<PostDbContext>().UseNpgsql(connection, optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure(5);
                optionsBuilder.CommandTimeout(500);
                optionsBuilder.MigrationsAssembly(typeof(PostDbContext).Assembly.FullName);
            }).UseSnakeCaseNamingConvention().UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        } 
        public PostDbContext CreateDbContext()
        {
            return new PostDbContext(_dbContextOptionsBuilder.Options);
        }
    }
    private PostgreSqlContainer PostgreSql { get; }
    public IDbContextFactory<PostDbContext> ContextFactory { get; private set; }

    public PostgresSqlSqlFixture()
    {
        this.PostgreSql = new PostgreSqlBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await this.PostgreSql.StartAsync()
            .ConfigureAwait(false);

        ContextFactory = new TestDbContextFactory(PostgreSql.GetConnectionString());
        await using var context = await ContextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await this.PostgreSql.DisposeAsync()
            .ConfigureAwait(false);
    }
}
