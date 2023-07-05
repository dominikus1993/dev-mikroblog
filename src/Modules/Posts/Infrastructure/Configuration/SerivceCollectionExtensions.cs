using DevMikroblog.BuildingBlocks.Infrastructure.Time;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevMikroblog.Modules.Posts.Infrastructure.Configuration;

public static class SerivceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<PostDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Posts")));

        return services;
    }
}