using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DevMikroblog.BuildingBlocks.Infrastructure.AspNetCore;

public class OpenTelemetryConfiguration
{
    public string? CollecttorUrl { get; set; }
    public string? ServiceName { get; set; }
    public string? ServiceVersion { get; set; }
    public bool OpenTelemetryEnabled { get; set; } = false;
    public bool OpenTelemetryLoggingEnabled { get; set; } = false;
    public bool OpenTelemetryMetricsEnabled { get; set; } = false;
}

public static class OpenTelemetryExtensions
{
    private static void SetupWithDefaults(OpenTelemetryConfiguration configuration)
    {
        configuration.CollecttorUrl ??= Environment.GetEnvironmentVariable("OTEL_COLLECTOR_URL");
        configuration.ServiceName ??= System.Environment.GetEnvironmentVariable("SERVICE_NAME") ??
                                      Assembly.GetExecutingAssembly().FullName;
        configuration.ServiceVersion ??= System.Environment.GetEnvironmentVariable("SERVICE_VERSION") ?? "v1.0.0";

        if (bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_ENABLED"), out var openTelemetryEnabled))
        {
            configuration.OpenTelemetryEnabled = openTelemetryEnabled;
        }
        if (bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_LOGGING_ENABLED"), out var otle))
        {
            configuration.OpenTelemetryLoggingEnabled = otle;
        }
        if (bool.TryParse(Environment.GetEnvironmentVariable("OPENTELEMETRY_METRICS_ENABLED"), out var ome))
        {
            configuration.OpenTelemetryMetricsEnabled = ome;
        }
    }

    public static WebApplicationBuilder AddOpenTelemetryTracing(this WebApplicationBuilder builder,
        Action<OpenTelemetryConfiguration>? configure = null, Action<TracerProviderBuilder>? setup = null)
    {
        var config = new OpenTelemetryConfiguration();
        configure?.Invoke(config);
        SetupWithDefaults(config);
        if (config.OpenTelemetryEnabled)
        {
            builder.Services.AddOpenTelemetryTracing(b =>
            {
                b.AddAspNetCoreInstrumentation();
                setup?.Invoke(b);
                b.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: config.ServiceName, serviceVersion: config.ServiceVersion));
                if (!string.IsNullOrEmpty(config.CollecttorUrl))
                {
                    b.AddOtlpExporter(options => options.Endpoint = new Uri(config.CollecttorUrl));
                }
            });
        }
        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryTracing(this WebApplicationBuilder builder,
        OpenTelemetryConfiguration? configure = null, Action<TracerProviderBuilder>? setup = null)
    {
        var config = configure ?? new OpenTelemetryConfiguration();
        SetupWithDefaults(config);
        if (config.OpenTelemetryEnabled)
        {
            builder.Services.AddOpenTelemetryTracing(b =>
            {
                b.AddAspNetCoreInstrumentation();
                setup?.Invoke(b);
                b.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: config.ServiceName, serviceVersion: config.ServiceVersion));
                if (!string.IsNullOrEmpty(config.CollecttorUrl))
                {
                    b.AddOtlpExporter(options => options.Endpoint = new Uri(config.CollecttorUrl));
                }
            });
        }
        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryLogging(this WebApplicationBuilder builder,
        Action<OpenTelemetryConfiguration>? configure = null, Action<OpenTelemetryLoggerOptions>? setup = null)
    {
        var config = new OpenTelemetryConfiguration();
        configure?.Invoke(config);
        SetupWithDefaults(config);
        if (config.OpenTelemetryLoggingEnabled)
        {
            builder.Logging.AddOpenTelemetry(b =>
            {
                b.IncludeFormattedMessage = true;
                b.IncludeScopes = true;
                b.ParseStateValues = true;
                setup?.Invoke(b);
                if (!string.IsNullOrEmpty(config.CollecttorUrl))
                {
                    b.AddOtlpExporter(options => options.Endpoint = new Uri(config.CollecttorUrl));
                }
            });
        }
        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryLogging(this WebApplicationBuilder builder,
        OpenTelemetryConfiguration? configure = null, Action<OpenTelemetryLoggerOptions>? setup = null)
    {
        var config = configure ?? new OpenTelemetryConfiguration();
        SetupWithDefaults(config);
        if (config.OpenTelemetryLoggingEnabled)
        {
            builder.Logging.AddOpenTelemetry(b =>
            {
                b.IncludeFormattedMessage = true;
                b.IncludeScopes = true;
                b.ParseStateValues = true;
                setup?.Invoke(b);
                if (!string.IsNullOrEmpty(config.CollecttorUrl))
                {
                    b.AddOtlpExporter(options => options.Endpoint = new Uri(config.CollecttorUrl));
                }
            });
        }
        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryMetrics(this WebApplicationBuilder builder,
        Action<OpenTelemetryConfiguration>? configure = null, Action<MeterProviderBuilder>? setup = null)
    {
        var config = new OpenTelemetryConfiguration();
        configure?.Invoke(config);
        SetupWithDefaults(config);
        if (config.OpenTelemetryMetricsEnabled)
        {
            builder.Services.AddOpenTelemetryMetrics(b =>
            {
                b.AddAspNetCoreInstrumentation();
                setup?.Invoke(b);
                if (!string.IsNullOrEmpty(config.CollecttorUrl))
                {
                    b.AddOtlpExporter(options => options.Endpoint = new Uri(config.CollecttorUrl));
                }
            });
        }
        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryMetrics(this WebApplicationBuilder builder,
        OpenTelemetryConfiguration? configure = null, Action<MeterProviderBuilder>? setup = null)
    {
        var config = configure ?? new OpenTelemetryConfiguration();
        SetupWithDefaults(config);
        if (config.OpenTelemetryMetricsEnabled)
        {
            builder.Services.AddOpenTelemetryMetrics(b =>
            {
                b.AddAspNetCoreInstrumentation();
                setup?.Invoke(b);
                if (!string.IsNullOrEmpty(config.CollecttorUrl))
                {
                    b.AddOtlpExporter(options => options.Endpoint = new Uri(config.CollecttorUrl));
                }
            });
        }
        return builder;
    }
}