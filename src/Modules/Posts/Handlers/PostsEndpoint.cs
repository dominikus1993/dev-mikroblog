using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;
using DevMikroblog.BuildingBlocks.Infrastructure.Modules;
using DevMikroblog.Modules.Posts.Application.PostCreator;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
using DevMikroblog.Modules.Posts.Application.PostProvider;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Infrastructure.Configuration;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using Marten;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevMikroblog.Modules.Posts.Handlers;

public class PostsEndpoint : IModule
{
    public static WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<GetPostsUseCase>();
        builder.Services.AddTransient<CreatePostUseCase>();
        builder.Services.AddTransient<IPostsReader, MartenPostReader>();
        builder.Services.AddTransient<IPostWriter, MartenPostWriter>();
        builder.Services.AddMarten(MartenDocumentStoreConfig.Configure(
            builder.Configuration.GetConnectionString("PostsDb"), builder.Environment.IsDevelopment()));
        builder.Services.AddPublisher<PostCreated>("posts", "created");
        return builder;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/posts", () => Post.CreateNew("x", new Author(AuthorId.New(), "")));

        return endpoints;
    }
}