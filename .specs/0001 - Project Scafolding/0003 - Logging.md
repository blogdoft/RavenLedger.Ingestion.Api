# 0003 - Logging

This system will use Serilog as log library.

Logs output must be on json format with template `"{ {date: @t, level: @l, message: @m, exception: @x, ..@p} }" + Environment.NewLine; `.

The Log setup must be called by a extension method, into `./src/RavenLedger.Ingest.Api/Observability/LogExtension.cs`.

Application must emit a Informational Log when starts and a Warning Log when shutdown, including version and date time utc-0.

Serilog should be used with Microsoft Logging.

When finished, add a commit with message: '0003 - Logging'