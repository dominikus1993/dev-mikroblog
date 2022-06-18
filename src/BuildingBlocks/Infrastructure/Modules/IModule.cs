using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Modules;

public interface IModule
{
    static abstract WebApplicationBuilder RegisterModule(WebApplicationBuilder builder);
    static abstract IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}