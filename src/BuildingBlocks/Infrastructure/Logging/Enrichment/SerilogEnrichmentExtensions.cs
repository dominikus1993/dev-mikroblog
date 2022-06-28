using Serilog;
using Serilog.Configuration;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Logging.Enrichment;

internal static class SerilogEnrichmentExtensions
{
    public static LoggerConfiguration WithCustomerId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration, nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<CustomerIdEnricher>();
    }
    
    public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<CorrelationIdEnricher>();
    }

    public static LoggerConfiguration WithCorrelationIdHeader(
        this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<CorrelationIdHeaderEnricher>();
    }
}