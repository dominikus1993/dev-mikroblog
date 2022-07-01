using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

using RabbitMQ.Client;

using Serilog;

[assembly: InternalsVisibleTo("Infrastructure.Tests")]
namespace DevMikroblog.BuildingBlocks.Infrastructure.Messaging.OpenTelemetry;

internal static class RabbitMqOpenTelemetry
{
    public const string RabbitMqOpenTelemetrySourceName = $"{nameof(DevMikroblog)}.Rabbitmq";
    
    public readonly static ActivitySource RabbitMqSource = new(RabbitMqOpenTelemetrySourceName, "v1.0.0");
    
    public static void AddActivityToHeader(Activity activity, IBasicProperties props)
    {
        var context = new PropagationContext(activity.Context, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(context, props,(properties, key, value) =>  InjectContextIntoHeader(properties, key, value));
    }
    
    public static PropagationContext GetHeaderFromProps(IBasicProperties props)
    {
        var context = new PropagationContext();
        return Propagators.DefaultTextMapPropagator.Extract(context, props, (properties, s) => ExtractContextFromHeader(properties, s));
    }

    public static void InjectContextIntoHeader(IBasicProperties props, string key, string value)
    {
        props.Headers ??= new Dictionary<string, object>();
        props.Headers[key] = value;
    }
    
    public static IEnumerable<string> ExtractContextFromHeader(IBasicProperties props, string key)
    {
        if (props.Headers is null)
        {
            return Enumerable.Empty<string>();
        }
        if(props.Headers.TryGetValue(key, out var value) && value is string result)
        {
            return new List<string>() { result };
        }
        return Enumerable.Empty<string>();
    }
}