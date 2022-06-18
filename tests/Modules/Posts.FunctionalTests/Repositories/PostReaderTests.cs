using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
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
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
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
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
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
    
    [Fact]
    public async Task GetPostTestsWhenExists()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var author = new Author(AuthorId.New(), "jan pawel 2");
        var post = Post.CreateNew("xDDD", author);
        var post2 = Post.CreateNew("xDDD", author);
        await writer.CreatePost(post);
        await writer.CreatePost(post2);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(2);
        subject.Should().Contain(x => x.Id == post.Id).And.Contain(x => x.Id == post2.Id);
    }
    
    [Fact]
    public async Task GetPostTestsWhenExistsAndPageSizeIsOneShouldReturnOneNewerPost()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var author = new Author(AuthorId.New(), "jan pawel 2");
        var post = Post.CreateNew("xDDD", author);
        var post2 = Post.CreateNew("xDDD", author);
        await writer.CreatePost(post);
        await writer.CreatePost(post2);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 1), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(1);
        subject.Should().Contain(x => x.Id == post2.Id);
    }
    
    [Fact]
    public async Task GetPostTestsWhenDbIsEmpty()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetPostWithTagsTestsWhenNotExists()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetPostWithTagTestsWhenExists()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var author = new Author(AuthorId.New(), "jan pawel 2");
        var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
        var post2 = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")} );
        await writer.CreatePost(post);
        await writer.CreatePost(post2);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(2);
        subject.Should().Contain(x => x.Id == post.Id).And.Contain(x => x.Id == post2.Id);
    }
    
    [Fact]
    public async Task GetPostWithTagTestsWhenOneExist()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var author = new Author(AuthorId.New(), "jan pawel 2");
        var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
        var post2 = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("csharp")} );
        await writer.CreatePost(post);
        await writer.CreatePost(post2);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(1);
        subject.Should().Contain(x => x.Id == post.Id);
    }
    
    [Fact]
    public async Task GetPostWithAuthorIDTestsWhenOneExist()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var author = new Author(AuthorId.New(), "jan pawel 2");
        var author2 = new Author(AuthorId.New(), "testoviron");
        var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
        var post2 = Post.CreateNew("xDDD", author2, new Tag[]{ new Tag("csharp")} );
        await writer.CreatePost(post);
        await writer.CreatePost(post2);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12, AuthorId: author2.Id), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(1);
        subject.Should().Contain(x => x.Id == post2.Id && x.Author == author2);
    }
    
    [Fact]
    public async Task GetPostWithAuthorIDTestsWhenNotExist()
    {
        // Arrange
        await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
        var reader = new MartenPostReader(_fixture.Store);
        var writer = new MartenPostWriter(_fixture.Store);
        var author = new Author(AuthorId.New(), "jan pawel 2");
        var author2 = new Author(AuthorId.New(), "testoviron");
        var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
        var post2 = Post.CreateNew("xDDD", author2, new Tag[]{ new Tag("csharp")} );
        await writer.CreatePost(post);
        await writer.CreatePost(post2);
        // Act
        var subject = await reader.GetPosts(new GetPostQuery(1, 12, AuthorId: AuthorId.New()), CancellationToken.None).ToListAsync();
        
        // Test
        subject.Should().NotBeNull();
        subject.Should().BeEmpty();
    }
}