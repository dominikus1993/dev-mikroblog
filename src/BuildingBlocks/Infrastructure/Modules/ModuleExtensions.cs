using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Modules;

public static class ModuleExtensions
{
    public static IEndpointRouteBuilder MapModule<T>(this IEndpointRouteBuilder app) where T : IModule
    {
        T.MapEndpoints(app);
        return app;
    }
    
    public static WebApplicationBuilder AddModule<T>(this WebApplicationBuilder services) where T : IModule
    {
        T.RegisterModule(services);
        return services;
    }
}