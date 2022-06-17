using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using FluentAssertions;

using LanguageExt.UnsafeValueAccess;

using Posts.FunctionalTests.Fixtures;

using Xunit;

namespace Posts.FunctionalTests.Repositories;

public class PostReaderTests : IClassFixture<PostgresSqlSqlFixture>
{
    private readonly PostgresSqlSqlFixture _fixture;

    public PostReaderTests(PostgresSqlSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPostByIdTestsWhenNotExists()
    {
        // Arrange
        var reader = new MartenPostReader(_fixture.Store);
        
        // Act
        var post = await reader.GetPostById(PostId.New());
        
        // Test
        post.IsSome.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetPostByIdTestsWhenExists()
    {
        // Arrange
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var post = Post.CreateNew("xDDD", new Author(AuthorId.New(), "jan pawel 2"), null);
        await writer.CreatePost(post);
        // Act
        var subject = await reader.GetPostById(post.Id);
        
        // Test
        subject.IsSome.Should().BeTrue();
        var result = subject.ValueUnsafe();
        result.Id.Should().Be(post.Id);
        result.Author.Should().Be(post.Author);
        result.Likes.Should().Be(0);
        result.ReplyTo.Should().BeNull();
        result.Content.Should().Be(post.Content);
    }
}