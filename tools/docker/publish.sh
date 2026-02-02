#!/bin/bash
set -euo pipefail

exit_code=0

# Ensure we're in workspace containing the solution/repo
cd /workspace || exit 1

# Clean, restore and build the solution
#dotnet clean || exit_code=$?
#dotnet restore || exit_code=$?
#dotnet build -c Release || exit_code=$?

find . -type d -name bin -exec rm -rf {} +
find . -type d -name obj -exec rm -rf {} +
find . -type d -name g -exec rm -rf {} +

# Copy solution README.md into each main project directory for NuGet packaging
cp /workspace/README.md /workspace/src/TirsvadCLI.Portfolio.Domain/README.md
cp /workspace/README.md /workspace/src/TirsvadCLI.Portfolio.Core/README.md
cp /workspace/README.md /workspace/src/TirsvadCLI.Portfolio.Infrastructure/README.md

# Remove existing LocalNuget source if present, then add
#dotnet nuget remove source LocalNuget || true
dotnet nuget add source /nuget --name nuget
#dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org

# Pack Domain first so Core can consume it (with symbols)
dotnet pack src/TirsvadCLI.Portfolio.Domain/TirsvadCLI.Portfolio.Domain.csproj -c Release -o /nuget --include-symbols --include-source || exit_code=$?

# Restore and build Core, referencing local NuGet source
dotnet restore src/TirsvadCLI.Portfolio.Core/TirsvadCLI.Portfolio.Core.csproj --source /nuget || exit_code=$?
dotnet build src/TirsvadCLI.Portfolio.Core/TirsvadCLI.Portfolio.Core.csproj -c Release || exit_code=$?

# Pack Core and Infrastructure (with symbols)
dotnet pack src/TirsvadCLI.Portfolio.Core/TirsvadCLI.Portfolio.Core.csproj -c Release -o /nuget --include-symbols --include-source || exit_code=$?
dotnet pack src/TirsvadCLI.Portfolio.Infrastructure/TirsvadCLI.Portfolio.Infrastructure.csproj -c Release -o /nuget --include-symbols --include-source || exit_code=$?

exit $exit_code
