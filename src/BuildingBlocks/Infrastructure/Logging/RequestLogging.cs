using System.Security.Claims;
using System.Security.Principal;

using DevMikroblog.BuildingBlocks.Infrastructure.Logging.Enrichment;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Logging;

public static class RequestLogging
{
    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is not null) // same as !(endpoint is null)
        {
            return string.Equals(
                endpoint.DisplayName,
                "Health checks",
                StringComparison.Ordinal);
        }

        // No endpoint, so not a health check endpoint
        return false;
    }

    private static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception? ex) =>
        ex is not null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                    ? LogEventLevel.Verbose // Was a health check, use Verbose
                    : LogEventLevel.Information;

    private static void EnrichFromRequest(
        IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;
        diagnosticContext.Set("Host", request.Host.ToString());
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        if (IsAuthenticatedRequest(httpContext.User.Identity))
        {
            var userId = GetUserId(httpContext.User);
            if (userId.HasValue)
            {
                diagnosticContext.Set("UserId", userId.Value);
            }
        }
        
        var endpoint = httpContext.GetEndpoint();
        if (endpoint != null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }

    private static bool IsAuthenticatedRequest(IIdentity? identity)
    {
        return identity?.IsAuthenticated ?? false;
    }

    private static Guid? GetUserId(ClaimsPrincipal user)
    {
        if (Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = EnrichFromRequest;
            opts.GetLevel = ExcludeHealthChecks; // Use the custom level
        });
        return app;
    }
}