# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

**RavenLedger.Ingestion.Api** is the ingestion API microservice for RavenLedger — an audit event storage system that guarantees tamper-proof evidence. It is part of the [Blog do FT](https://github.com/blogdoft/BlogDoFT.RavenLedger) technical blog series, which documents building this product from scratch in Brazilian Portuguese.

## Product context

RavenLedger receives, stores, and chains audit events so that internal data manipulation can be detected and proven. Target users are court-appointed auditors and internal compliance officers.

**It is / does:**
- Audit event ingestion and storage
- Chains records for tamper detection (hash-based linking)
- Identifies entity versions and authors
- Passively detects internal data violations

**It is NOT / does NOT:**
- Observability/tracing system
- Fraud prevention or determination
- Event replay or data correction
- Operational/financial auditing

## Event schema

Incoming events follow [CloudEvents 1.0](https://cloudevents.io/) as the envelope, with this `data` payload structure:

```json
{
  "appInfo": { "appname", "domain", "version" },
  "user":    { "id", "name", "role" },
  "operation": { "type", "entity", "generatedAt", "transmittedAt" },
  "index":   { /* custom searchable fields */ },
  "data":    { /* entity snapshot */ }
}
```

`operation.type` values: `insert`, `update`, `delete`.

## Build, test, and run

> This section will be populated once the initial project structure is committed. Expected .NET commands:

```bash
dotnet build
dotnet test
dotnet run --project src/RavenLedger.Ingestion.Api
```

To run a single test:
```bash
dotnet test --filter "FullyQualifiedName~TestClassName"
```
