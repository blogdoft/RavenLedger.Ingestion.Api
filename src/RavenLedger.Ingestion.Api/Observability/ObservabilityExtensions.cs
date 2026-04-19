using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace RavenLedger.Ingestion.Api.Observability;

public static class ObservabilityExtensions
{
    public static IHostApplicationBuilder AddObservability(this IHostApplicationBuilder builder)
    {
        var options = builder.Configuration
            .GetSection(ObservabilityOptions.SectionName)
            .Get<ObservabilityOptions>() ?? new ObservabilityOptions();

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(o =>
                        o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/metrics"))
                    .AddHttpClientInstrumentation(o => o.RecordException = true);

                if (options.Otlp.Enabled)
                {
                    tracing.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(options.Otlp.Endpoint);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    });
                }

                if (options.Zipkin.Enabled)
                {
                    tracing.AddZipkinExporter(o =>
                        o.Endpoint = new Uri(options.Zipkin.Endpoint));
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (options.Otlp.Enabled)
                {
                    metrics.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(options.Otlp.Endpoint);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    });
                }

                if (options.Prometheus.Enabled)
                {
                    metrics.AddPrometheusExporter();
                }
            });

        return builder;
    }

    public static IApplicationBuilder UseObservability(this WebApplication app)
    {
        var options = app.Configuration
            .GetSection(ObservabilityOptions.SectionName)
            .Get<ObservabilityOptions>() ?? new ObservabilityOptions();

        if (options.Prometheus.Enabled)
        {
            app.UseOpenTelemetryPrometheusScrapingEndpoint(
                options.Prometheus.ScrapeEndpointPath);
        }

        return app;
    }
}
