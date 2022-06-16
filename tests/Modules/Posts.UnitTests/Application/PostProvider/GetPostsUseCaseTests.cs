using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DevMikroblog.Modules.Posts.Application.PostProvider;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;

using FluentAssertions;

using Moq;

using Xunit;

using GetPostQuery = DevMikroblog.Modules.Posts.Domain.Repositories.GetPostQuery;

namespace Posts.UnitTests.Core.UseCases;

public class GetPostsUseCaseTests
{
    [Fact]
    public async Task TestGetPostsWhenThereAreNoPosts_ShouldBeEmptyList()
    {
        // Arrange
        var mock = new Mock<IPostsReader>();
        mock
            .Setup(provider => provider.GetPosts(It.IsAny<GetPostQuery>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncEnumerable.Empty<Post>());

        var useCase = new GetPostsUseCase(mock.Object);
        // Act

        var query = new GetPostsQuery(1, 2);
        var result = await useCase.Execute(query, CancellationToken.None);

        var subject = result.ToList();
        // Test
        subject.Should().BeEmpty();
        mock.Verify(x => x.GetPosts(It.Is<GetPostQuery>(x => x.Page == query.Page && x.PageSize == query.PageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task TestGetPostsWhenProviderReturnSomePosts_ShouldBeFullfilledList()
    {
        // Arrange
        var data = new List<Post>()
        {
            new(PostId.New(), "test", null, DateTime.UtcNow, new Author(AuthorId.New(), "xD"), 2)
        };
        var mock = new Mock<IPostsReader>();
        mock
            .Setup(provider => provider.GetPosts(It.IsAny<GetPostQuery>(), It.IsAny<CancellationToken>()))
            .Returns(data.ToAsyncEnumerable());

        var useCase = new GetPostsUseCase(mock.Object);
        
        // Act
        var query = new GetPostsQuery(1, 2);
        var result = await useCase.Execute(query, CancellationToken.None);

        var subject = result.ToList();
        // Test
        subject.Should().HaveCount(data.Count);
        subject.Should().Contain(x => x.PostId == data[0].Id.Value);
        
        mock.Verify(x => x.GetPosts(It.Is<GetPostQuery>(x => x.Page == query.Page && x.PageSize == query.PageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
}