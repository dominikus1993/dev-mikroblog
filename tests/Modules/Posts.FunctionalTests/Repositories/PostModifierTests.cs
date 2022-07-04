using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using FluentAssertions;

using LanguageExt.UnsafeValueAccess;

using Posts.FunctionalTests.Fixtures;

using Xunit;

namespace Posts.FunctionalTests.Repositories;

public class PostModifierTests: IClassFixture<PostgresSqlSqlFixture>
{
    private readonly PostgresSqlSqlFixture _fixture;

    public PostModifierTests(PostgresSqlSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPostDetailsTestsWhenNotExists()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var modifier = new MartenPostModifier(_fixture.Store);
        // Act
        var postId = PostId.New();
        await modifier.Modify(postId, x => x.IncrementRepliesQuantity(),default);
        var post = await reader.GetPostById(postId, CancellationToken.None);
        // Test
        post.IsSome.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetPostDetailsTestsWhenPostExists()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var writer = new MartenPostWriter(_fixture.Store);
        var reader = new MartenPostReader(_fixture.Store);
        var modifier = new MartenPostModifier(_fixture.Store);
        // Act
        var post = Post.CreateNew("xDDD", new Author(AuthorId.New(), "jan pawel 2"), null);
        await writer.Save(post);
        await modifier.Modify(post.Id, x => x.IncrementRepliesQuantity(),default);
        var subject = await reader.GetPostById(post.Id, CancellationToken.None);
        // Test
        subject.IsSome.Should().BeTrue();
        var result = subject.ValueUnsafe();
        result.Id.Should().Be(post.Id);
        result.Content.Should().Be(post.Content);
        result.Author.Should().Be(post.Author);
        result.Tags.IsNone.Should().BeTrue();
        result.RepliesQuantity.Should().Be(post.RepliesQuantity + 1);
    }
}