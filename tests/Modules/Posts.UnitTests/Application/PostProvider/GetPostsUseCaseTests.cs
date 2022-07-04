using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DevMikroblog.Modules.Posts.Application.PostProvider;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using FluentAssertions;

using LanguageExt;
using LanguageExt.UnsafeValueAccess;

using Moq;

using Xunit;

using GetPostQuery = DevMikroblog.Modules.Posts.Domain.Repositories.GetPostQuery;
using static LanguageExt.Prelude;
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
            .Returns(() => Task.FromResult<Option<PagedPosts>>(None));

        var useCase = new GetPostsUseCase(mock.Object);
        // Act

        var query = new GetPostsQuery(1, 2);

        var subject = await useCase.Execute(query, CancellationToken.None);
        // Test
        subject.IsNone.Should().BeTrue();
        mock.Verify(x => x.GetPosts(It.Is<GetPostQuery>(x => x.Page == query.Page && x.PageSize == query.PageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task TestGetPostsWhenProviderReturnSomePosts_ShouldBeFullfilledList()
    {
        // Arrange
        var data = new List<Post>()
        {
            new(PostId.New(), "test", null, DateTime.UtcNow, new Author(AuthorId.New(), "xD"), null, 2, 0 )
        };
        var mock = new Mock<IPostsReader>();
        mock
            .Setup(provider => provider.GetPosts(It.IsAny<GetPostQuery>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<Option<PagedPosts>>(new PagedPosts(data, 1, data.Count)));

        var useCase = new GetPostsUseCase(mock.Object);
        
        // Act
        var query = new GetPostsQuery(1, 2);
        var result = await useCase.Execute(query, CancellationToken.None);
        result.IsSome.Should().BeTrue();
        var subject = result.ValueUnsafe();
        // Test
        subject.Posts.Should().HaveCount(data.Count);
        subject.Posts.Should().Contain(x => x.PostId == data[0].Id.Value);
        
        mock.Verify(x => x.GetPosts(It.Is<GetPostQuery>(x => x.Page == query.Page && x.PageSize == query.PageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
}