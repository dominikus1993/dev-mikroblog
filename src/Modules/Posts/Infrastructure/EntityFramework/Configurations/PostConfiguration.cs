using DevMikroblog.Modules.Posts.Domain.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevMikroblog.Modules.Posts.Infrastructure.EntityFramework.Configurations;

public sealed class PostIdConverter : ValueConverter<PostId, Guid>
{
    public PostIdConverter()
        : base(
            v => v.Value,
            v => new PostId(v))
    {
    }
}

public sealed class AuthorIdConverter : ValueConverter<AuthorId, Guid>
{
    public AuthorIdConverter()
        : base(
            v => v.Value,
            v => new AuthorId(v))
    {
    }
}

public sealed class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    public DateTimeOffsetConverter()
        : base(
            v => DateTime.SpecifyKind(v.UtcDateTime, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Local))

    {
    }
}

public sealed class PostConfiguration: IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<PostIdConverter>();
        builder.Property(x => x.Tags)
            .HasColumnType("jsonb");
        builder
            .HasIndex(b => new { b.Tags })
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");
        builder.OwnsOne<Author>(x => x.Author, b => b.Property(x => x.Id).HasConversion<AuthorIdConverter>());
        builder.OwnsOne(x => x.ReplyTo, b => b.Property(x => x.Id).HasConversion<PostIdConverter>());
        builder.Property(x => x.CreatedAt).HasConversion<DateTimeOffsetConverter>();
    }
}