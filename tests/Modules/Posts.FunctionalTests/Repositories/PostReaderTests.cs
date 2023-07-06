using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using FluentAssertions;

using Posts.FunctionalTests.Fixtures;

using Xunit;

namespace Posts.FunctionalTests.Repositories;

public class PostReaderTests : IClassFixture<PostgresSqlSqlFixture>, IDisposable
{
    private readonly PostgresSqlSqlFixture _fixture;
    private readonly PostDbContext _postDbContext;
    private readonly IPostsReader _postsReader;
    public PostReaderTests(PostgresSqlSqlFixture fixture)
    {
        _fixture = fixture;
        _postDbContext = _fixture.ContextFactory.CreateDbContext();
        _postsReader = new EntityPostRepository(_postDbContext);
    }

    [Theory]
    [AutoData]
    public async Task GetPostDetailsTestsWhenNotExists(PostId postId)
    {
        // Act
        var post = await _postsReader.GetPostDetails(postId, default);
        
        // Test
        post.Should().BeNull();
    }
    
    [Theory]
    [AutoData]
    public async Task GetPostByIdTestWhenNotExists(PostId postId)
    {
        // Act
        var post = await _postsReader.GetPostById(postId, default);
        
        // Test
        post.Should().BeNull();
    }
    
    // [Fact]
    // public async Task GetPostDetailsTestsWhenExistsAndHasNoParentsOrReplies()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var post = Post.CreateNew("xDDD", new Author(AuthorId.New(), "jan pawel 2"), null);
    //     await writer.Save(post);
    //     // Act
    //     var subject = await reader.GetPostDetails(post.Id, CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var details = subject.ValueUnsafe();
    //     
    //     details.ReplyTo.IsNone.Should().BeTrue();
    //     details.Replies.IsNone.Should().BeTrue();
    //     details.Post.Id.Should().Be(post.Id);
    //     details.Post.Author.Should().Be(post.Author);
    //     details.Post.Likes.Should().Be(0);
    //     details.Post.ReplyTo.Should().BeNull();
    //     details.Post.Content.Should().Be(post.Content);
    // }
    //
    // [Fact]
    // public async Task GetPostDetailsTestsWhenExistsAndHasNoParentsAndHasReplies()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var post = Post.CreateNew("xDDD", new Author(AuthorId.New(), "jan pawel 2"), null);
    //     await writer.Save(post);
    //     var post2 = Post.CreateNew("xDDD2", new Author(AuthorId.New(), "jan pawel 2"), null, new ReplyToPost(post.Id));
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPostDetails(post.Id, CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var details = subject.ValueUnsafe();
    //     details.Post.Id.Should().Be(post.Id);
    //     details.Post.Author.Should().Be(post.Author);
    //     details.Post.Likes.Should().Be(0);
    //     details.Post.ReplyTo.Should().BeNull();
    //     details.Post.Content.Should().Be(post.Content);
    //     details.Replies.IsSome.Should().BeTrue();
    //     var replies = details.Replies.ValueUnsafe();
    //     replies.Should().NotBeEmpty();
    //     replies.Should().HaveCount(1);
    //     replies.Should().Contain(x => x.Id == post2.Id);
    // }
    //
    // [Fact]
    // public async Task GetPostDetailsTestsWhenExistsAndHasParentsAndHasReplies()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var parentPost = Post.CreateNew("xDDD", new Author(AuthorId.New(), "jan pawel 2"), null);
    //     await writer.Save(parentPost);      
    //     var post = Post.CreateNew("xDDD", new Author(AuthorId.New(), "jan pawel 2"), null, new ReplyToPost(parentPost.Id));
    //     await writer.Save(post);
    //     var post2 = Post.CreateNew("xDDD2", new Author(AuthorId.New(), "jan pawel 2"), null, new ReplyToPost(post.Id));
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPostDetails(post.Id, CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var details = subject.ValueUnsafe();
    //     details.Post.Id.Should().Be(post.Id);
    //     details.Post.Author.Should().Be(post.Author);
    //     details.Post.Likes.Should().Be(0);
    //     details.Post.ReplyTo.Should().Be(new ReplyToPost(parentPost.Id));
    //     details.Post.Content.Should().Be(post.Content);
    //     details.Replies.IsSome.Should().BeTrue();
    //     var replies = details.Replies.ValueUnsafe();
    //     replies.Should().NotBeEmpty();
    //     replies.Should().HaveCount(1);
    //     replies.Should().Contain(x => x.Id == post2.Id);
    //     details.ReplyTo.IsSome.Should().BeTrue();
    //     var parent = details.ReplyTo.ValueUnsafe();
    //     parent.Id.Should().Be(parentPost.Id);
    // }
    //
    // [Fact]
    // public async Task GetPostTestsWhenExists()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var post = Post.CreateNew("xDDD", author, None);
    //     var post2 = Post.CreateNew("xDDD", author, None);
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var data = subject.ValueUnsafe();
    //     data.Should().NotBeNull();
    //     data.Posts.Should().NotBeEmpty();
    //     data.Posts.Should().HaveCount(2);
    //     data.Posts.Should().Contain(x => x.Id == post.Id).And.Contain(x => x.Id == post2.Id);
    //     data.TotalPages.Should().Be(1);
    //     data.TotalPostsQuantity.Should().Be(2);
    // }
    //
    // [Fact]
    // public async Task GetPostTestsWhenExistsAndPageSizeIsOneShouldReturnOneNewerPost()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var post = Post.CreateNew("xDDD", author, None);
    //     var post2 = Post.CreateNew("xDDD", author, None);
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 1), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var data = subject.ValueUnsafe();
    //     data.Should().NotBeNull();
    //     data.Posts.Should().NotBeEmpty();
    //     data.Posts.Should().HaveCount(1);
    //     data.Posts.Should().Contain(x => x.Id == post2.Id);
    //     data.TotalPages.Should().Be(2);
    //     data.TotalPostsQuantity.Should().Be(2);
    // }
    //
    // [Fact]
    // public async Task GetPostTestsWhenDbIsEmpty()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     // 
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsNone.Should().BeTrue();
    // }
    //
    // [Fact]
    // public async Task GetPostWithTagsTestsWhenNotExists()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsNone.Should().BeTrue();
    // }
    //
    // [Fact]
    // public async Task GetPostWithTagTestsWhenExists()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
    //     var post2 = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")} );
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var data = subject.ValueUnsafe();
    //     data.Posts.Should().NotBeNull();
    //     data.Posts.Should().NotBeEmpty();
    //     data.Posts.Should().HaveCount(2);
    //     data.Posts.Should().Contain(x => x.Id == post.Id).And.Contain(x => x.Id == post2.Id);
    //     data.TotalPages.Should().Be(1);
    //     data.TotalPostsQuantity.Should().Be(2);
    // }
    //
    // [Fact]
    // public async Task GetPostWithTagTestsWhenOneExist()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
    //     var post2 = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("csharp")} );
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var data = subject.ValueUnsafe();
    //     data.Should().NotBeNull();
    //     data.Posts.Should().NotBeEmpty();
    //     data.Posts.Should().HaveCount(1);
    //     data.Posts.Should().Contain(x => x.Id == post.Id);
    //     data.TotalPages.Should().Be(1);
    //     data.TotalPostsQuantity.Should().Be(1);
    // }
    //
    // [Fact]
    // public async Task GetPostWithAuthorIDTestsWhenOneExist()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var author2 = new Author(AuthorId.New(), "testoviron");
    //     var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
    //     var post2 = Post.CreateNew("xDDD", author2, new Tag[]{ new Tag("csharp")} );
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12, AuthorId: author2.Id), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsSome.Should().BeTrue();
    //     var data = subject.ValueUnsafe();
    //     data.Should().NotBeNull();
    //     data.Posts.Should().NotBeEmpty();
    //     data.Posts.Should().HaveCount(1);
    //     data.Posts.Should().Contain(x => x.Id == post2.Id && x.Author == author2);
    //     data.TotalPages.Should().Be(1);
    //     data.TotalPostsQuantity.Should().Be(1);
    // }
    //
    // [Fact]
    // public async Task GetPostWithAuthorIDTestsWhenNotExist()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var author2 = new Author(AuthorId.New(), "testoviron");
    //     var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
    //     var post2 = Post.CreateNew("xDDD", author2, new Tag[]{ new Tag("csharp")} );
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12, AuthorId: AuthorId.New()), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsNone.Should().BeTrue();
    // }
    //
    //
    // [Fact]
    // public async Task TestReadPostsReplies()
    // {
    //     // Arrange
    //     await _fixture.Store.Advanced.Clean.CompletelyRemoveAllAsync();
    //     var reader = new MartenPostReader(_fixture.Store);
    //     var writer = new MartenPostWriter(_fixture.Store);
    //     var author = new Author(AuthorId.New(), "jan pawel 2");
    //     var author2 = new Author(AuthorId.New(), "testoviron");
    //     var post = Post.CreateNew("xDDD", author, new Tag[]{ new Tag("fsharp")});
    //     var post2 = Post.CreateNew("xDDD", author2, new Tag[]{ new Tag("csharp")} );
    //     await writer.Save(post);
    //     await writer.Save(post2);
    //     // Act
    //     var subject = await reader.GetPosts(new GetPostQuery(1, 12, AuthorId: AuthorId.New()), CancellationToken.None);
    //     
    //     // Test
    //     subject.IsNone.Should().BeTrue();
    // }
    public void Dispose()
    {
        _postDbContext?.Dispose();
    }
}