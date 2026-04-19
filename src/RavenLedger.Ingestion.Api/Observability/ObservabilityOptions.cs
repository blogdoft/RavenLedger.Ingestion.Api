namespace RavenLedger.Ingestion.Api.Observability;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public OtlpOptions Otlp { get; init; } = new();

    public ZipkinOptions Zipkin { get; init; } = new();

    public PrometheusOptions Prometheus { get; init; } = new();
}
