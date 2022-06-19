using System.Diagnostics;

using DevMikroblog.BuildingBlocks.Infrastructure.Logging;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;
using DevMikroblog.Modules.Posts.Handlers;

using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.UseLogging("DevMikroblog.Api");
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRabbitMq(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
PostsEndpoint.RegisterModule(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("tye"))
{
    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();

app.UseAuthorization();

app.MapControllers();
PostsEndpoint.MapEndpoints(app);

app.Run();
