using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using FluentAssertions;

using LanguageExt.UnsafeValueAccess;

using Posts.FunctionalTests.Fixtures;

using Xunit;

namespace Posts.FunctionalTests.Repositories;

public class PostModifierTests: IClassFixture<PostgresSqlSqlFixture>
{
    private readonly PostgresSqlSqlFixture _fixture;
    private readonly IPostsReader _postsReader;
    private readonly IPostModifier _postModifier;
    private readonly IPostWriter _postWriter;

    public PostModifierTests(PostgresSqlSqlFixture fixture)
    {
        _fixture = fixture;
        _postsReader = new MartenPostReader(_fixture.Context);
        _postModifier = new EfCorePostModifier(_fixture.Context);
        _postWriter = new MartenPostWriter(_fixture.Context);
    }

    [Theory]
    [AutoData]
    public async Task GetPostDetailsTestsWhenNotExists(PostId postId)
    {
        // Act
        await _postModifier.Modify(postId, x => x.IncrementRepliesQuantity(),default);
        var post = await _postsReader.GetPostById(postId, CancellationToken.None);
        // Test
        post.IsSome.Should().BeFalse();
    }
    
    [Theory]
    [AutoData]
    public async Task GetPostDetailsTestsWhenPostExists(Post post)
    {
        // Act
        await _postWriter.Save(post);
        await _postModifier.Modify(post.Id, x => x.IncrementRepliesQuantity(),default);
        var subject = await _postsReader.GetPostById(post.Id, CancellationToken.None);
        // Test
        subject.IsSome.Should().BeTrue();
        var result = subject.ValueUnsafe();
        result.Id.Should().Be(post.Id);
        result.Content.Should().Be(post.Content);
        result.Author.Should().BeEquivalentTo(post.Author);
        result.Tags.Should().BeEquivalentTo(post.Tags);
        result.RepliesQuantity.Should().Be(post.RepliesQuantity + 1);
        result.Version.Should().Be(post.Version + 1);
    }
}