using System.Diagnostics;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.IoC;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

using RabbitMQ.Client;

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
        return Propagators.DefaultTextMapPropagator.Extract(context, props, (properties, s) => InjectContextIntoHeader(properties, s));
    }

    private static void InjectContextIntoHeader(IBasicProperties props, string key, string value)
    {
        props.Headers ??= new Dictionary<string, object>();
        props.Headers[key] = value;
    }
    
    private static IEnumerable<string> InjectContextIntoHeader(IBasicProperties props, string key)
    {
        if (props.Headers is null)
        {
            yield break;
        }
        
        if(props.Headers.TryGetValue(key, out var value) && value is string header)
        {
            yield return header;
        }
    }
}