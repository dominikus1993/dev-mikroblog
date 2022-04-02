using Serilog;
using Serilog.Configuration;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Logging.Enrichment;

public static class CustomerInfoConfigurationExtensions
{
    public static LoggerConfiguration WithCustomerId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration, nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<CustomerIdEnricher>();
    }
}