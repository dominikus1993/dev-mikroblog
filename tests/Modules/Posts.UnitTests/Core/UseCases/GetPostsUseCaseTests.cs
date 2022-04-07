using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DevMikroblog.Modules.Posts.Core.Model;
using DevMikroblog.Modules.Posts.Core.Providers;
using DevMikroblog.Modules.Posts.Core.UseCases;

using FluentAssertions;

using Moq;

using Xunit;

namespace Posts.UnitTests.Core.UseCases;

public class GetPostsUseCaseTests
{
    [Fact]
    public async Task TestGetPostsWhenThereAreNoPosts_ShouldBeEmptyList()
    {
        // Arrange
        var mock = new Mock<IPostsProvider>();
        mock
            .Setup(provider => provider.Provide(It.IsAny<GetPostsQuery>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncEnumerable.Empty<Post>());

        var useCase = new GetPostsUseCase(mock.Object);
        
        // Act

        var result = await useCase.Execute(new GetPostsQuery(1, 2), CancellationToken.None);

        var subject = result.ToList();
        // Test

        subject.Should().BeEmpty();

    }
}