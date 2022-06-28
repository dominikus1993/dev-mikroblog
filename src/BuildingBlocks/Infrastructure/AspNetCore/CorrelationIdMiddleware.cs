using DevMikroblog.BuildingBlocks.Infrastructure.Configuration;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DevMikroblog.BuildingBlocks.Infrastructure.AspNetCore;


public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CorrelationIdOptions _options;
    
    public CorrelationIdMiddleware(RequestDelegate next, IOptions<CorrelationIdOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _next = next ?? throw new ArgumentNullException(nameof(next));

        _options = options.Value;
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdOptions.DefaultHeader, out StringValues correlationId))
        {
            context.TraceIdentifier = correlationId;
        }

        if (_options.IncludeInResponse)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(CorrelationIdOptions.DefaultHeader, context.TraceIdentifier);
                return Task.CompletedTask;
            });
        }

        return _next(context);
    }
}