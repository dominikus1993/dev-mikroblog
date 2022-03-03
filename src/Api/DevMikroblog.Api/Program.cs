using DevMikroblog.BuildingBlocks.Infrastructure.Logging;

using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.UseLogging("DevMikroblog.Api");
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
