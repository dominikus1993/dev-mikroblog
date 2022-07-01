using System.Diagnostics;
using System.Text.Json;

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
        Console.WriteLine("inject" + key + " " + value);
        props.Headers ??= new Dictionary<string, object>();
        props.Headers[key] = value;
    }
    
    private static IEnumerable<string> InjectContextIntoHeader(IBasicProperties props, string key)
    {
        Console.WriteLine("XXXX " + key);
        Console.WriteLine(props.Headers.Exists(x => x.Key == key));
        if (props.Headers is null)
        {
            yield break;
        }

        var get = props.Headers.TryGetValue(key, out var value);
        Console.WriteLine(get  + " and is string " + value is string);
        if(get && value is string result)
        {
            Console.WriteLine("Result " + result);
            yield return result;
        }
    }
}