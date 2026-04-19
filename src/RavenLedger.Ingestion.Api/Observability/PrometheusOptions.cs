namespace RavenLedger.Ingestion.Api.Observability;

public sealed class PrometheusOptions
{
    public bool Enabled { get; init; }

    public string ScrapeEndpointPath { get; init; } = "/metrics";
}
