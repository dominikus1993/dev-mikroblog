// using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Abstractions;
// using DevMikroblog.Modules.Posts.Application.PostCreator;
// using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
// using DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;
// using DevMikroblog.Modules.Posts.Domain.Model;
// using DevMikroblog.Modules.Posts.Domain.Repositories;
//
// using LanguageExt;
//
// using Moq;
//
// using Xunit;
//
// namespace Posts.UnitTests.Application.PostCreator;
//
// public class CreatePostUseCaseTests
// {
//     [Fact]
//     public async Task TestCreateNewPost()
//     {
//         // Arrange 
//         var author = new Author(AuthorId.New(), "jan pawel 2");
//         var post = new CreatePostCommand(author, "xDDDD", null);
//         
//         var writerMock = new Mock<IPostWriter>();
//         writerMock.Setup(writer => writer.Save(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
//             .Returns(Task.CompletedTask);
//         var publisherMock = new Mock<IMessagePublisher<PostCreated>>();
//         publisherMock.Setup(x => x.Publish(It.IsAny<PostCreated>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(Unit.Default);
//         var parserMock = new Mock<IPostTagParser>();
//         parserMock.Setup(x => x.ParseTagsFromPostContent(It.IsAny<string>()));
//
//         var useCase = new CreatePostUseCase(writerMock.Object, publisherMock.Object, parserMock.Object);
//         
//         // Act
//         await useCase.Execute(post, CancellationToken.None);
//         
//         // Test
//         parserMock.Verify(x => x.ParseTagsFromPostContent(It.IsAny<string>()), Times.Once);
//         writerMock.Verify(x => x.Save(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
//         publisherMock.Verify(x => x.Publish(It.IsAny<PostCreated>(), It.IsAny<CancellationToken>()), Times.Once);
//     }
//     
//     [Fact]
//     public async Task TestCreateNewPostWhenWriterHasError()
//     {
//         // Arrange 
//         var author = new Author(AuthorId.New(), "jan pawel 2");
//         var post = new CreatePostCommand(author, "xDDDD", null);
//         
//         var writerMock = new Mock<IPostWriter>();
//         writerMock.Setup(writer => writer.Save(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
//             .Throws(new Exception("Error"));
//         var publisherMock = new Mock<IMessagePublisher<PostCreated>>();
//         publisherMock.Setup(x => x.Publish(It.IsAny<PostCreated>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(Unit.Default);
//         var parserMock = new Mock<IPostTagParser>();
//         parserMock.Setup(x => x.ParseTagsFromPostContent(It.IsAny<string>()));
//         var useCase = new CreatePostUseCase(writerMock.Object, publisherMock.Object, parserMock.Object);
//         
//         // Act
//         
//         await Assert.ThrowsAsync<Exception>(() => useCase.Execute(post, CancellationToken.None));
//         
//         // Test
//         parserMock.Verify(x => x.ParseTagsFromPostContent(It.IsAny<string>()), Times.Once);
//         writerMock.Verify(x => x.Save(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
//         publisherMock.Verify(x => x.Publish(It.IsAny<PostCreated>(), It.IsAny<CancellationToken>()), Times.Never);
//     }
// }