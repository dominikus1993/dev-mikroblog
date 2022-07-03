using DevMikroblog.Modules.Posts.Domain.Model;

using LanguageExt;

namespace DevMikroblog.Modules.Posts.Domain.Repositories;

public interface IPostModifier
{
    Task<Unit> Modify(PostId postId, Func<Post, Post> modifyF,
        CancellationToken cancellationToken);
}