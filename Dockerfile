# Multi-stage Dockerfile consolidating build, test-runner, exporter and runtime stages.
# Usage:
#  - Build artifacts image: docker build --target builder -t portfolio-builder:ci .
#  - Run exporter to copy artifacts to host: docker build --target exporter -t portfolio-exporter:ci . && docker run --rm -v $(pwd)/artifacts:/artifacts portfolio-exporter:ci
#  - Run tests: docker build --target test-runner -t portfolio-test-runner:ci . && docker run --rm -v $(pwd):/workspace -v $(pwd)/TestResults:/artifacts/TestResults portfolio-test-runner:ci
#  - Build runtime image: docker build --target runtime -t portfolio:latest .

################################################################################
# Base SDK image for building and test-runner
################################################################################
#mcr.microsoft.com/dotnet/sdk
FROM tirsvad/tirsvadcli_debian13_nginx:latest AS builder
#FROM mcr.microsoft.com/dotnet/sdk:latest AS builder
SHELL ["/bin/bash", "-lc"]
#SHELL ["/bin/bash"]
WORKDIR /src
ARG CONFIGURATION=Release

# Copy source into the image
COPY ./ /src/

#RUN wget https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.101/dotnet-sdk-10.0.101-linux-x64.tar.gz \
    #&& mkdir -p /usr/share/dotnet \
    #&& tar -zxf dotnet-sdk-10.0.101-linux-x64.tar.gz -C /usr/share/dotnet
#

# Install wasm-tools workload
#RUN dotnet workload install wasm-tools --skip-manifest-update

# Restore and build the main web project
RUN dotnet restore ./src/TirsvadCLI.Portfolio.Core/TirsvadCLI.Portfolio.Core.csproj && \
    dotnet build ./src/TirsvadCLI.Portfolio.Core/TirsvadCLI.Portfolio.Core.csproj -c Debug --no-restore

# Also build any other projects under /src so their build outputs (bin/obj) are produced
#RUN set -eux; \
    #find ./src -name '*.csproj' -print0 | xargs -0 -n1 -I{} dotnet build "{}" -c $CONFIGURATION --no-restore || true
#
# Publish the web project into /artifacts
#RUN dotnet publish ./src/Portfolio/Portfolio.csproj -c $CONFIGURATION -o /artifacts --no-build  || true
#RUN dotnet publish ./src/Portfolio/Portfolio.csproj -c $CONFIGURATION -o /artifacts

################################################################################
# Test-runner stage: runtime container that executes tests when started
################################################################################
FROM tirsvad/tirsvadcli_debian13_nginx:latest AS test-runner
SHELL ["/bin/bash", "-lc"]
WORKDIR /workspace

# Ensure environment variables are available inside the test-runner container
#ENV ASPNETCORE_ENVIRONMENT=Development \
    #DOCKER_DOTNET_TEST=true
    #DB_PORTFOLIO_HOST=host.docker.internal

# Copy the repo so tests can run without relying on a mounted workspace (optional)
COPY ./ /workspace

# Ensure entrypoint script exists to run tests
COPY tools/docker/run_tests.sh /run_tests.sh
RUN chmod +x /run_tests.sh || true

ENTRYPOINT ["/run_tests.sh"]

################################################################################
# Runtime stage: production runtime that serves the published app
################################################################################
FROM tirsvad/tirsvadcli_debian13_nginx:latest AS publish
SHELL ["/bin/bash", "-lc"]
WORKDIR /workspace

# Copy the repo so tests can run without relying on a mounted workspace (optional)
COPY --from=builder /src /workspace

RUN mkdir /nuget

# Pack the main project to generate NuGet package(s)
#RUN dotnet pack ./src/TirsvadCLI.Portfolio.Core/TirsvadCLI.Portfolio.Core.csproj -c Release -o /nuget

COPY tools/docker/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh || true

ENTRYPOINT ["bash", "/entrypoint.sh"]

################################################################################
# Exporter stage: copies NuGet packages for host export
################################################################################
FROM publish AS exporter
COPY --from=publish /nuget /nuget
