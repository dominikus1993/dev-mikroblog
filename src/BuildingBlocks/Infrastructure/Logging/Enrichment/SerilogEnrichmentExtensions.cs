using Serilog;
using Serilog.Configuration;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Logging.Enrichment;

internal static class SerilogEnrichmentExtensions
{
    public static LoggerConfiguration WithCustomerId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration, nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<UserIdEnricher>();
    }
}