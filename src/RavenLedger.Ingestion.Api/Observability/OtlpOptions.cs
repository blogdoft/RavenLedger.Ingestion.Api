namespace RavenLedger.Ingestion.Api.Observability;

public sealed class OtlpOptions
{
    public bool Enabled { get; init; }

    public string Endpoint { get; init; } = "http://localhost:4317";
}
