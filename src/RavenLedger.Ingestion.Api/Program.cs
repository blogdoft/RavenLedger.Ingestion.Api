using System.Reflection;
using RavenLedger.Ingestion.Api.Observability;
using Serilog;
using Serilog.Formatting.Compact;

var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console(new CompactJsonFormatter()));

builder.Services.AddOpenApi();
builder.AddObservability();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseObservability();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application started. Version: {Version}, StartedAt: {StartedAt:O}", version, DateTime.UtcNow);
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning("Application shutting down. Version: {Version}, ShutdownAt: {ShutdownAt:O}", version, DateTime.UtcNow);
});

await app.RunAsync();
