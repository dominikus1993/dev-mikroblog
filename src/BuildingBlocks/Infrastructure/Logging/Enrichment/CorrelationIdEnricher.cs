using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Serilog.Core;
using Serilog.Events;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Logging.Enrichment;

internal class CorrelationIdEnricher : ILogEventEnricher
{
    private const string CorrelationIdPropertyName = "CorrelationId";
    private const string CorrelationIdItemName = $"{nameof(CorrelationIdEnricher)}+CorrelationId";
    private readonly IHttpContextAccessor _contextAccessor;

    public CorrelationIdEnricher() : this(new HttpContextAccessor())
    {
    }

    internal CorrelationIdEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_contextAccessor.HttpContext is null)
            return;

        var correlationId = GetCorrelationId(_contextAccessor.HttpContext);

        var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

        logEvent.AddOrUpdateProperty(correlationIdProperty);
    }

    private string GetCorrelationId(HttpContext context)
    {
        return (string) (context.Items[CorrelationIdItemName] ??
                         (context.Items[CorrelationIdItemName] = Guid.NewGuid().ToString()));
    }
}

internal class CorrelationIdHeaderEnricher : ILogEventEnricher
{
    private const string CorrelationIdPropertyName = "CorrelationId";
    private readonly string _headerKey;
    private readonly IHttpContextAccessor _contextAccessor;
    private const string HeaderKey = "x-correlation-id";

    public CorrelationIdHeaderEnricher() : this(HeaderKey)
    {
        
    }
    public CorrelationIdHeaderEnricher(string headerKey) : this(headerKey, new HttpContextAccessor())
    {
    }

    internal CorrelationIdHeaderEnricher(string headerKey, IHttpContextAccessor contextAccessor)
    {
        _headerKey = headerKey;
        _contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_contextAccessor.HttpContext == null)
            return;

        var correlationId = GetCorrelationId(_contextAccessor.HttpContext);

        var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

        logEvent.AddOrUpdateProperty(correlationIdProperty);
    }

    private static string? FirstOrDefault(StringValues values)
    {
        if (values.Count > 0)
        {
            return values[0];

        }
        return default;
    }

    private string GetCorrelationId(HttpContext context)
    {
        var header = string.Empty;

        if (context.Request.Headers.TryGetValue(_headerKey, out var values))
        {
            header = FirstOrDefault(values);
        }
        else if (context.Response.Headers.TryGetValue(_headerKey, out values))
        {
            header = FirstOrDefault(values);
        }

        var correlationId = string.IsNullOrEmpty(header)
            ? Guid.NewGuid().ToString()
            : header;

        if (!context.Response.Headers.ContainsKey(_headerKey))
        {
            context.Response.Headers.Add(_headerKey, correlationId);
        }

        return correlationId;
    }
}