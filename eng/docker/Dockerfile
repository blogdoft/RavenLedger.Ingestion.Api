# Auto-generated optimized Dockerfile for .NET 9

# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# Copy only solution and project files (preserving structure) to leverage caching
COPY RavenLedger.Ingestion.Api.sln RavenLedger.Ingestion.Api.sln
COPY __tests__/RavenLedger.Ingestion.Api.Tests/RavenLedger.Ingestion.Api.Tests.csproj __tests__/RavenLedger.Ingestion.Api.Tests/RavenLedger.Ingestion.Api.Tests.csproj
COPY src/RavenLedger.Ingestion.Api/RavenLedger.Ingestion.Api.csproj src/RavenLedger.Ingestion.Api/RavenLedger.Ingestion.Api.csproj

# Restore dependencies with locked file
RUN dotnet restore --use-lock-file

# Copy all remaining source code
COPY . ./

# Publish trimmed release build
RUN dotnet publish src/RavenLedger.Ingestion.Api/RavenLedger.Ingestion.Api.csproj \
    -c Release \
    --self-contained false \
    --no-restore \
    -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Install CA certificates for HTTPS
RUN apk add --no-cache ca-certificates

# Copy published output
COPY --from=builder /app/publish ./

# Expose default port and set entrypoint
EXPOSE 80
ENTRYPOINT ["dotnet", "RavenLedger.Ingestion.Api.dll"]
