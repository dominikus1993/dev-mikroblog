using System;
using System.Threading.Tasks;

using DevMikroblog.Modules.Posts.Infrastructure.EfCore;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Posts.UnitTests.Fixtures;

public sealed class PostgresSqlSqlFixture : IAsyncLifetime, IDisposable
{
    private readonly TestcontainerDatabaseConfiguration configuration = new PostgreSqlTestcontainerConfiguration("postgres:14-alpine") { Database = "recommendations", Username = "postgres", Password = "postgres" };

    public PostgreSqlTestcontainer Container { get; }
    public PostsDbContext PostsDbContext { get; private set; }

    public PostgresSqlSqlFixture()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        this.Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(this.configuration)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await this.Container.StartAsync()
            .ConfigureAwait(false);
        var builder = new DbContextOptionsBuilder().UseNpgsql(this.Container.ConnectionString, optionsBuilder =>
        {
            optionsBuilder.EnableRetryOnFailure(5);
            optionsBuilder.CommandTimeout(500);
            optionsBuilder.MigrationsAssembly("Posts");
        }).UseSnakeCaseNamingConvention();
        this.PostsDbContext = new PostsDbContext(builder.Options);
        await this.PostsDbContext.Database.MigrateAsync();

    }

    public async Task DisposeAsync()
    {
        await this.Container.DisposeAsync()
            .ConfigureAwait(false);
        //await this.RecommendationsDbContext.DisposeAsync();
    }

    public void Dispose()
    {
        this.configuration.Dispose();
    }
}