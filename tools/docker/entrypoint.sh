#!/bin/bash﻿
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

# Pack all main projects
dotnet pack -c Release -o /nuget || exit_code=$?


exit $exit_code
