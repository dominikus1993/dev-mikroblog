using System.Runtime.CompilerServices;

using DevMikroblog.Modules.Posts.Domain.Model;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Repositories;

internal interface IPostWriter
{
    Task CreatePost(Post post, CancellationToken cancellationToken = default);
}