using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMikroblog.Modules.Posts.Infrastructure.EfCore.Configuration;

internal class EfPostConfiguaration : IEntityTypeConfiguration<EfPost>
{
    public void Configure(EntityTypeBuilder<EfPost> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasOne(x => x.ReplyTo).WithOne().HasForeignKey<EfPost>(x => x.ReplyToPostId);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.CreatedAt);
    }
}