using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Domain.Repositories;

public record PostDetails(Post Post, Post? ReplyTo);