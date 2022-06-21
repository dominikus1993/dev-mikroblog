using System.Diagnostics;
using System.Text;

using DevMikroblog.BuildingBlocks.Infrastructure.Logging;
using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;
using DevMikroblog.Modules.Posts.Handlers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.UseLogging("DevMikroblog.Api");
// Add services to the container.

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
