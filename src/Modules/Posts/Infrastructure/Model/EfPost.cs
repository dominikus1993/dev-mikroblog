namespace DevMikroblog.Modules.Posts.Infrastructure.Model;

internal class EfPost
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
}