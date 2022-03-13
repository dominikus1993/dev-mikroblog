using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevMikroblog.Modules.Posts.Infrastructure.EfCore.Configuration;

internal class EfPostConfiguaration : IEntityTypeConfiguration<EfPost>
{
    public void Configure(EntityTypeBuilder<EfPost> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Content).IsRequired();
    }
}