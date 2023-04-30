using System.Runtime.CompilerServices;

using DevMikroblog.Modules.Posts.Domain.Model;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace DevMikroblog.Modules.Posts.Domain.Repositories;

public interface IPostWriter
{
    Task Add(Post post, CancellationToken cancellationToken = default);
    Task Update(Post post, CancellationToken cancellationToken = default);
}