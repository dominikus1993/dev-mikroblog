using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Extensions;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using FluentAssertions;

using Posts.FunctionalTests.Fixtures;

using Xunit;

namespace Posts.FunctionalTests.Infrastructure;

public sealed class QueryableExtensionsTests : IClassFixture<PostgresSqlSqlFixture>, IAsyncLifetime
{
    private readonly PostgresSqlSqlFixture _postgresSqlSqlFixture;
    private readonly PostDbContext _postDbContext;

    public QueryableExtensionsTests(PostgresSqlSqlFixture postgresSqlSqlFixture)
    {
        _postgresSqlSqlFixture = postgresSqlSqlFixture;
        _postDbContext = _postgresSqlSqlFixture.ContextFactory.CreateDbContext();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    public async Task TEstWhenPageNumberIsEqualOrSmallerThan0(int pageNumber)
    {

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _postDbContext.Posts.AsQueryable().ToPagedListAsync(pageNumber, 1));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    public async Task TEstWhenPageSizeIsEqualOrSmallerThan0(int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _postDbContext.Posts.AsQueryable().ToPagedListAsync(1, pageSize));
    }
    
    [Theory]
    [AutoData]
    public async Task TestWhenRecordsNotExists()
    {
        var subject = await _postDbContext.Posts.AsQueryable().ToPagedListAsync(1, 1);
        
        subject.Should().NotBeNull();
        subject.IsEmpty.Should().BeTrue();
    }
    
    [Theory]
    [AutoData]
    public async Task TestWhenRecordsExists(EfPost[] posts)
    {
        await _postgresSqlSqlFixture.Seed(posts);

        var subject = await _postDbContext.Posts.AsQueryable().ToPagedListAsync(1, 1);

        subject.IsEmpty.Should().BeFalse();
        subject.Should().NotBeNull();
        subject.PageCount.Should().Be(posts.Length);
        subject.TotalItemCount.Should().Be(posts.Length);
        subject.Items.Should().NotBeEmpty();
        subject.Items.Should().HaveCount(1);
    }
    
    [Theory]
    [AutoData]
    public async Task TestWhenRecordsExistsAndPageSizeIsGreaterThanDbSize(EfPost[] posts)
    {
        await _postgresSqlSqlFixture.Seed(posts);

        var subject = await _postDbContext.Posts.AsQueryable().ToPagedListAsync(1, posts.Length + 1);

        subject.Should().NotBeNull();
        subject.IsEmpty.Should().BeFalse();
        subject.PageCount.Should().Be(1);
        subject.TotalItemCount.Should().Be(posts.Length);
        subject.Items.Should().NotBeEmpty();
        subject.Items.Should().HaveCount(posts.Length);
    }
    
    public async Task InitializeAsync()
    {
        await _postgresSqlSqlFixture.Clean();
    }

    public async Task DisposeAsync()
    {
        await _postDbContext.DisposeAsync();
    }
}