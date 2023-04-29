using DevMikroblog.BuildingBlocks.Infrastructure.Auth;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;
using DevMikroblog.BuildingBlocks.Infrastructure.Modules;
using DevMikroblog.BuildingBlocks.Infrastructure.Time;
using DevMikroblog.Modules.Posts.Application.PostCreator;
using DevMikroblog.Modules.Posts.Application.PostCreator.Events;
using DevMikroblog.Modules.Posts.Application.PostCreator.Handlers;
using DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;
using DevMikroblog.Modules.Posts.Application.PostProvider;
using DevMikroblog.Modules.Posts.Domain.Model;
using DevMikroblog.Modules.Posts.Domain.Repositories;
using DevMikroblog.Modules.Posts.Handlers.Requests;
using DevMikroblog.Modules.Posts.Infrastructure.EntityFramework;
using DevMikroblog.Modules.Posts.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevMikroblog.Modules.Posts.Handlers;

public static class PostModuleExtensions
{
    public static IHealthChecksBuilder AddPostsModuleHealthChecks(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        return builder.AddNpgSql(configuration.GetConnectionString("PostDb"));
    }
}

public sealed class PostsModule : IModule
{
    public static WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<GetPostsUseCase>();
        builder.Services.AddTransient<CreatePostUseCase>();
        builder.Services.AddTransient<GetPostDetailsUseCase>();
        builder.Services.AddTransient<IPostsReader, MartenPostReader>();
        builder.Services.AddTransient<IPostWriter, MartenPostWriter>();
        builder.Services.AddTransient<IPostModifier, EfCorePostModifier>();
        builder.Services.AddTransient<IPostTagParser, PostTagParser>();
        builder.Services.AddPublisher<PostCreated>("posts", "created");
        builder.Services.AddSubscriber<PostCreated, PostCreatedHandler>("posts", "created");
        builder.Services.AddSingleton<ISystemClock, UtcSystemClock>();
        builder.Services
            .AddDbContextPool<PostDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostsDb"), optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure(5);
                    optionsBuilder.CommandTimeout(500);
                    optionsBuilder.MigrationsAssembly(typeof(PostDbContext).Assembly.FullName);
                }).UseSnakeCaseNamingConvention();
            });
        return builder;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/posts", GetPosts);
        endpoints.MapPost("/post", CreatePost)
            .RequireAuthorization();
        endpoints.MapGet("/post/{postId}", GetPostById);
        return endpoints;
    }

    private static async Task<IResult> GetPosts(GetPostsRequest request, GetPostsUseCase useCase,
        CancellationToken cancellationToken)
    {
        AuthorId? authorId = request.AuthorId is null ? null : new AuthorId(Guid.Parse(request.AuthorId));
        var result = await useCase.Execute(new GetPostsQuery(request.Page, request.PageSize, request.Tag, authorId), cancellationToken);
        return result.Match<IResult>(posts => Results.Ok(posts), () => Results.NotFound());
    }
    
    private static async Task<IResult> GetPostById(Guid postId, GetPostDetailsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.Execute(new PostId(postId), cancellationToken);

        return result.Match(x => Results.Ok(x), () => Results.NotFound());
    }
    
    [Authorize]
    private static async Task<IResult> CreatePost(HttpContext context, CreatePostRequest request, CreatePostUseCase useCase,
        CancellationToken cancellationToken)
    {
        var authorId = new AuthorId(context.User.UserId());
        var author = new Author(authorId, context.User.UserName());
        ReplyToPost? replyToPost = request.ReplyToPostId.HasValue ? new ReplyToPost(new PostId(request.ReplyToPostId.Value)) : null;
        var command =
            new CreatePostCommand(author, request.Content, replyToPost);
        await useCase.Execute(command, cancellationToken);
        
        return Results.Ok();
    }
}