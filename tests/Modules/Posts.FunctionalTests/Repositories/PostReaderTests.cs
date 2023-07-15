using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using FluentAssertions;

using Posts.FunctionalTests.Fixtures;

using Xunit;

namespace Posts.FunctionalTests.Repositories;

public class PostReaderTests : IClassFixture<PostgresSqlSqlFixture>, IDisposable, IAsyncLifetime
{
    private readonly PostgresSqlSqlFixture _fixture;
    private readonly PostDbContext _postDbContext;
    private readonly IPostsReader _postsReader;
    private readonly IPostWriter _postWriter;
    public PostReaderTests(PostgresSqlSqlFixture fixture)
    {
        _fixture = fixture;
        _postDbContext = _fixture.ContextFactory.CreateDbContext();
        _postsReader = new EntityPostRepository(_postDbContext);
        _postWriter = new EntityPostRepository(_postDbContext);
    }
    
    
    [Theory]
    [AutoData]
    public async Task GetPostByIdTestWhenNotExists(PostId postId)
    {
        // Act
        var post = await _postsReader.GetPostById(postId);

        // Test
        post.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task GetPostByIdTestWhenExists(Post post)
    {
        // Arrange 
        await _postWriter.Add(post);

        // Act
        var postById = await _postsReader.GetPostById(post.Id);

        // Test
        postById.Should().NotBeNull();
        postById.Should().BeEquivalentTo(post, x => x.Excluding(p => p.CreatedAt));
    }

    [Theory]
    [AutoData]
    public async Task GetPostByIdTestWhenExistsAndUpdatePost(Post post)
    {
        // Arrange 
        await _postWriter.Add(post);

        var oldLikesQ = post.Likes;

        post.IncrementRepliesQuantity();

        await _postWriter.Update(post);
        // Act
        var postById = await _postsReader.GetPostById(post.Id);

        // Test
        postById.Should().NotBeNull();
        postById.Should().BeEquivalentTo(post, x => x.Excluding(p => p.CreatedAt).Excluding(p => p.DeletedAt));
        postById.Likes.Should().Be(oldLikesQ + 1);
    }

    [Theory]
    [AutoData]
    public async Task GetPostDetailsTestsWhenExistsAndHasNoParentsOrReplies(Post post)
    {
        //Arrange
        await _postWriter.Add(post);
        // Act
        var subject = await _postsReader.GetPostDetails(post.Id, CancellationToken.None);
        
        // Test
        subject.Should().NotBeNull();

        subject.Should().BeEquivalentTo(subject, x => x);
    }
    
    [Theory]
    [AutoData]
    public async Task GetPostDetailsTestsWhenExistsAndHasNoParentsAndHasReplies(Post post, Post reply)
    {
        // Arrange
        reply.AddReplyTo(post.Id);
        await _postWriter.Add(post);
        await _postWriter.Add(reply);
        // Act
        var subject = await _postsReader.GetPostDetails(reply.Id, CancellationToken.None);
        
        // Test
        Assert.NotNull(subject);
        subject.Post.Should().BeEquivalentTo(reply, x => x.Excluding(p => p.CreatedAt).Excluding(p => p.DeletedAt));
        subject.ReplyTo.Should().BeEquivalentTo(post, x => x.Excluding(p => p.CreatedAt).Excluding(p => p.DeletedAt));
    }
    
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
    
    [Fact]
    public async Task GetPostWithTagsTestsWhenNotExists()
    {
        // Arrange
        await _fixture.Clean();
 
        // Act
        var subject = await _postsReader.GetPosts(new GetPostQuery(1, 12, "fsharp"), CancellationToken.None);
        
        // Test
        subject.Should().BeNull();
    }
    
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

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _fixture.Clean();
    }
}