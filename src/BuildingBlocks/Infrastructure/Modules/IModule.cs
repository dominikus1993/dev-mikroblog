using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Modules;

public interface IModule
{
    static abstract WebApplicationBuilder RegisterModule(WebApplicationBuilder builder);
    static abstract IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}