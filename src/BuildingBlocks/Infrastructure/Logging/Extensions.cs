using System.Reflection;

using DevMikroblog.BuildingBlocks.Infrastructure.Logging.Enrichment;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Logging;

public class SerilogOptions
{
    public bool ConsoleEnabled { get; set; } = true;
    public string MinimumLevel { get; set; } = "Information";
    public string Format { get; set; } = "compact";
    public Dictionary<string, string>? Override { get; set; }

    public LogEventLevel GetMinimumLogEventLevel()
    {
        if (!Enum.TryParse<LogEventLevel>(MinimumLevel, true, out var level))
        {
            level = LogEventLevel.Information;
        }
        return level;
    }

    public static SerilogOptions Empty => new();
}

public static class Extensionss
{
    private readonly static Dictionary<string, string> DefaultOverride = new() { { "Microsoft.AspNetCore", "Warning" } };
    private static Dictionary<string, string> BindOverride(Dictionary<string, string>? o)
    {
        if (o is null)
        {
            return DefaultOverride;
        }
        foreach (var (key, value) in DefaultOverride)
        {
            o.TryAdd(key, value);
        }
        return o;
    }

    private static LogEventLevel ParseLevel(string level)
    {
        if (Enum.TryParse<LogEventLevel>(level, true, out var lvl))
        {
            return lvl;
        }
        return LogEventLevel.Information;
    }

    public static WebApplicationBuilder UseLogging(this WebApplicationBuilder builder, string? applicationName = null)
    {
        builder.Host.UseLogging(applicationName);
        return builder;
    }


    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, string? applicationName = null)
    {
        string? appName = applicationName ?? Assembly.GetExecutingAssembly().FullName;
        return hostBuilder.UseSerilog((context, configuration) =>
        {
            var serilogOptions = context.Configuration.GetSection("Serilog").Get<SerilogOptions>();
            var level = serilogOptions.GetMinimumLogEventLevel();

            var conf = configuration
                .MinimumLevel.Is(level)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("ApplicationName", appName)
                .Enrich.WithClientAgent()
                .Enrich.WithCustomerId()
                .Enrich.WithExceptionDetails();

            foreach (var (name, lvl) in BindOverride(serilogOptions.Override))
            {
                conf.MinimumLevel.Override(name, ParseLevel(lvl));
            }

            if (serilogOptions.ConsoleEnabled)
            {
                conf.WriteTo.Async((logger) =>
                {

                    switch (serilogOptions.Format.ToLower())
                    {
                        case "elasticsearch":
                            logger.Console(new ElasticsearchJsonFormatter());
                            break;
                        case "compact":
                            logger.Console(new RenderedCompactJsonFormatter());
                            break;
                        case "colored":
                            logger.Console(theme: AnsiConsoleTheme.Code);
                            break;
                        default:
                            logger.Console(new RenderedCompactJsonFormatter());
                            break;
                    }
                });
            }
        });
    }
}
