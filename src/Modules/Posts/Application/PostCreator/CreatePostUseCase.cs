using DevMikroblog.Modules.Posts.Domain.Repositories;

namespace DevMikroblog.Modules.Posts.Application.PostCreator;

public readonly record struct CreatePostCommand(string Content);

public sealed class CreatePostUseCase
{
    private readonly IPostWriter _postWriter;

    public Task Execute(CreatePostCommand command, CancellationToken cancellationToken)
    {
        
    }
}