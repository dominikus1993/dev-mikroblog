namespace DevMikroblog.Modules.Posts.Domain.Model;

public sealed record Author
{
    public Author(AuthorId Id, string? Name)
    {
        this.Id = Id;
        this.Name = Name;
    }

    public Author()
    {
        
    }
    public AuthorId Id { get; init; }
    public string? Name { get; init; }

    public void Deconstruct(out AuthorId Id, out string? Name)
    {
        Id = this.Id;
        Name = this.Name;
    }
}