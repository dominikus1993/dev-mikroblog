using System.Runtime.CompilerServices;

using PostCreator.Domain.Model;

[assembly: InternalsVisibleTo("Posts.UnitTests")]
[assembly: InternalsVisibleTo("Posts.FunctionalTests")]
namespace PostCreator.Domain.Repositories;

internal interface IPostWriter
{
    Task Save(Post post, CancellationToken cancellationToken = default);
}