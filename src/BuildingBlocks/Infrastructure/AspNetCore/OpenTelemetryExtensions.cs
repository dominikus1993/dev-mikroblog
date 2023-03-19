using System.Reflection;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.Publisher;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DevMikroblog.BuildingBlocks.Infrastructure.AspNetCore;

public class OpenTelemetryConfiguration
{
    private bool _openTelemetryEnabled = false;
    private bool _openTelemetryLoggingEnabled = false;
    private bool _openTelemetryMetricsEnabled = false;
    private bool _oltpExporterEnabled = false;
    private bool _consoleExporterEnabled = false;

    public bool OpenTelemetryEnabled
    {
        get =>
            bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_ENABLED"),
                out var openTelemetryEnabled) ? openTelemetryEnabled : _openTelemetryEnabled;
        set => _openTelemetryEnabled = value;
    }

    public bool OpenTelemetryLoggingEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_LOGGING_ENABLED"), out var otle) ? otle : _openTelemetryLoggingEnabled;
        set => _openTelemetryLoggingEnabled = value;
    }

    public bool OpenTelemetryMetricsEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_METRICS_ENABLED"), out var ome) ? ome : _openTelemetryMetricsEnabled;
        set => _openTelemetryMetricsEnabled = value;
    }

    public bool OltpExporterEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_OLTP_EXPORTER_ENABLED"), out var ome) ? ome : _oltpExporterEnabled;
        set => _oltpExporterEnabled = value;
    }

    public bool ConsoleExporterEnabled
    {
        get => bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_CONSOLE_EXPORTER_ENABLED"), out var ome) ? ome : _consoleExporterEnabled;
        set => _consoleExporterEnabled = value;
    }
}

public readonly record struct Tag(string Name, string Value);
public sealed class Service
{
    public IReadOnlyCollection<Tag>? Tags { get; init; } = null!;
    public string Name { get; init; } = Assembly.GetExecutingAssembly().FullName!;
    public string Version { get; init; } = "Unspecified";
    
    public OpenTelemetryConfiguration OpenTelemetryConfiguration { get; init; } = new();
}

public static class ResourceBuilderExtensions
{
    private static IEnumerable<KeyValuePair<string, object>> GetAttributes(this Service service, string envName)
    {
        yield return new KeyValuePair<string, object>("env", envName);
        if (service.Tags is { Count: > 0})
        {
            foreach ((string key, string value) in service.Tags)
            {
                yield return new KeyValuePair<string, object>(key, value);
            }
        }
    }
    
    public static ResourceBuilder GetResourceBuilder(this Service service, string envName)
    {
        return ResourceBuilder.CreateDefault()
            .AddTelemetrySdk()
            .AddService(serviceName: service.Name, serviceVersion: service.Version, serviceInstanceId: Environment.MachineName )
            .AddAttributes(GetAttributes(service, envName));
    }
}


public sealed class SampleOpenTelemetryBuilder
{
    public SampleOpenTelemetryBuilder(OpenTelemetryBuilder openTelemetryBuilder, IHostEnvironment environment,
        Service service)
    {
        OpenTelemetryBuilder = openTelemetryBuilder;
        Environment = environment;
        Service = service;
    }

    public Service Service { get; }

    public OpenTelemetryBuilder OpenTelemetryBuilder
    {
        get;
    }

    public IHostEnvironment Environment
    {
        get;
    }
}

public static class OpenTelemetryExtensions
{
    public static SampleOpenTelemetryBuilder AddOpenTelemetry(this WebApplicationBuilder builder,
        Service? configure = null)
    {
        var config = configure ?? new Service();
        var otelBuilder = builder.Services.AddOpenTelemetry();
        return new SampleOpenTelemetryBuilder(otelBuilder, builder.Environment, config);
    }

    public static SampleOpenTelemetryBuilder AddOpenTelemetryTracing(this SampleOpenTelemetryBuilder builder,
        Action<TracerProviderBuilder>? setup = null)
    {
        if (builder.Service.OpenTelemetryConfiguration.OpenTelemetryEnabled)
        {
            builder.OpenTelemetryBuilder.WithTracing(b =>
            {
                b.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                });
                setup?.Invoke(b);
                b.SetResourceBuilder(builder.Service.GetResourceBuilder(builder.Environment.EnvironmentName));
                if (builder.Service.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }
                //
                // if (builder.Service.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                // {
                //     b.AddConsoleExporter();
                // }
            });
        }

        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryLogging(this WebApplicationBuilder builder,
        Service? configure = null, Action<OpenTelemetryLoggerOptions>? setup = null)
    {
        var config = configure ?? new Service();
        if (config.OpenTelemetryConfiguration.OpenTelemetryLoggingEnabled)
        {
            builder.Logging.AddOpenTelemetry(b =>
            {
                b.IncludeFormattedMessage = true;
                b.IncludeScopes = true;
                b.ParseStateValues = true;
                b.AttachLogsToActivityEvent();
                b.SetResourceBuilder(config.GetResourceBuilder(builder.Environment.EnvironmentName));
                setup?.Invoke(b);
                if (config.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }

                // if (config.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                // {
                //     b.AddConsoleExporter();
                // }
            });
        }

        return builder;
    }

    public static SampleOpenTelemetryBuilder AddOpenTelemetryMetrics(this SampleOpenTelemetryBuilder builder,
        Action<MeterProviderBuilder>? setup = null)
    {
        if (builder.Service.OpenTelemetryConfiguration!.OpenTelemetryMetricsEnabled)
        {
            builder.OpenTelemetryBuilder.WithMetrics(b =>
            {
                b.AddAspNetCoreInstrumentation();
                b.SetResourceBuilder(builder.Service.GetResourceBuilder(builder.Environment.EnvironmentName));
                setup?.Invoke(b);
                if (builder.Service.OpenTelemetryConfiguration.OltpExporterEnabled)
                {
                    b.AddOtlpExporter();
                }

                // if (builder.Service.OpenTelemetryConfiguration.ConsoleExporterEnabled)
                // {
                //     b.AddConsoleExporter();
                // }
            });
        }

        return builder;
    }
}