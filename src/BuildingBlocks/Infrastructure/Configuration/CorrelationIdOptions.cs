namespace DevMikroblog.BuildingBlocks.Infrastructure.Configuration;

public class CorrelationIdOptions
{
    internal const string DefaultHeader = "x-correlation-id";
    
    public bool IncludeInResponse { get; set; } = true;
}