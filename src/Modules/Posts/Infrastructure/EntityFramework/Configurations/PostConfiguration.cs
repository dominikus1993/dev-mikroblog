using DevMikroblog.Modules.Posts.Domain.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Configurations;

internal sealed class PostIdConverter : ValueConverter<PostId, Guid>
{
    public PostIdConverter()
        : base(
            v => v.Value,
            v => new PostId(v))
    {
    }
}

internal sealed class AuthorIdConverter : ValueConverter<AuthorId, Guid>
{
    public AuthorIdConverter()
        : base(
            v => v.Value,
            v => new AuthorId(v))
    {
    }
}

public sealed class PostConfiguration: IEntityTypeConfiguration<Post>
{
    private const string RowVersion = nameof(RowVersion);

    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<PostIdConverter>();
        builder.Property(x => x.Tags)
            .HasColumnType("jsonb");
        builder
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english",  // Text search config
                p => new { p.Tags })  // Included properties
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN"); // Index method on the search vector (GIN or GIST)

        builder.OwnsOne<Author>(x => x.Author, b => b.Property(x => x.Id).HasConversion<AuthorIdConverter>());
        builder.OwnsOne(x => x.ReplyTo, b => b.Property(x => x.Id).HasConversion<PostIdConverter>());
        builder.Property(x => x.CreatedAt).HasConversion<DateTimeOffsetToBinaryConverter>();
        builder.Property(x => x.DeletedAt).HasConversion<DateTimeOffsetToBinaryConverter>();
        builder.Property<byte[]>(RowVersion)
            .IsRowVersion();
    }
}