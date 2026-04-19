#!/usr/bin/env bash
set -euo pipefail

# Usage: ./gen-dockerfile.sh [DIR] [DOCKERFILE]
# Generates an optimized Dockerfile for .NET 9,
# preserving directory structure of .sln and .csproj files.
# If DOCKERFILE is not provided, defaults to 'Dockerfile'.

SEARCH_DIR="${1:-.}"
DOCKERFILE="${2:-Dockerfile}"

# Validate directory
if [[ ! -d "$SEARCH_DIR" ]]; then
  echo "Error: '$SEARCH_DIR' is not a directory." >&2
  exit 1
fi

# Switch to the project directory
pushd "$SEARCH_DIR" > /dev/null

# Prepare COPY commands preserving directory structure: 'COPY <src> <src>'
COPY_SLN_CMDS=$( 
  find . -type f -name '*.sln' -print \
    | sed 's|^\./||' \
    | while IFS= read -r file; do printf 'COPY %s %s
' "$file" "$file"; done
)
COPY_CSPROJ_CMDS=$( 
  find . -type f -name '*.csproj' -print \
    | sed 's|^\./||' \
    | while IFS= read -r file; do printf 'COPY %s %s
' "$file" "$file"; done
)

# Return to original directory
popd > /dev/null

# Create the Dockerfile
cat > "$DOCKERFILE" <<EOF
# Auto-generated optimized Dockerfile for .NET 9

# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# Copy only solution and project files (preserving structure) to leverage caching
EOF

# Append the COPY commands for sln and csproj
{
  printf "%s\n" "$COPY_SLN_CMDS"
  printf "%s\n" "$COPY_CSPROJ_CMDS"
} >> "$DOCKERFILE"

# Continue Dockerfile
cat >> "$DOCKERFILE" <<'EOF'

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
EOF

echo "Dockerfile generated at: $DOCKERFILE"
