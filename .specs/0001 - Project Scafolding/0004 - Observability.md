# 0004 - Observability

Add and configure OpenTelemetry.

Create a specific folder for Observability on project root. 
Create DTO to read Open telemetry options from appSettings.json.
Create extension methods to read and configure OpenTelemetry. This Extension method should be called on `Program.cs`.

You should propose connection with Zipkin and prometheus and enable OTLP.q1