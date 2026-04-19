namespace RavenLedger.Ingestion.Api.Observability;

public sealed class ZipkinOptions
{
    public bool Enabled { get; init; }

    public string Endpoint { get; init; } = "http://localhost:9411/api/v2/spans";
}
