using System.Diagnostics;
using System.Text;
using DevMikroblog.BuildingBlocks.Infrastructure.Logging;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;
using DevMikroblog.BuildingBlocks.Infrastructure.Modules;
using DevMikroblog.Modules.Posts.Handlers;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);

builder.UseLogging("DevMikroblog.Api");
// Add services to the container.
builder.AddModule<PostsModule>();
builder.Services.AddControllers();
builder.Services.AddRabbitMq(builder.Configuration); 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    config.IncludeErrorDetails = true;
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"])),
    };
});


builder.Services.AddHealthChecks().AddRabbitMq().AddPostsModuleHealthChecks(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("tye"))
{
    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapModule<PostsModule>();

app.MapHealthChecks("/health",
    new HealthCheckOptions() { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.MapHealthChecks("/ping",
    new HealthCheckOptions() { Predicate = r => r.Name.Contains("self"), ResponseWriter = PongWriteResponse, });
app.Run();


static Task PongWriteResponse(HttpContext httpContext,
    HealthReport result)
{
    httpContext.Response.ContentType = "application/json";
    return httpContext.Response.WriteAsync("{\"pong\": \"message\"}");
}
