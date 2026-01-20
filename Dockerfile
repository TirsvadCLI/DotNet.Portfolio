################################################################################
# Base SDK image for building and test-runner
################################################################################
FROM tirsvad/tirsvadcli_debian13_nginx:latest AS builder
SHELL ["/bin/bash"]
WORKDIR /src

COPY ./ /src/

RUN ["dotnet", "restore"]
RUN ["dotnet", "build", "-c", "Release", "--no-restore"]

################################################################################
# Tests stage: runtime container that executes tests when started
################################################################################
FROM tirsvad/tirsvadcli_debian13_nginx:latest AS tests
SHELL ["/bin/bash"]
WORKDIR /workspace

COPY ./ /workspace

COPY tools/docker/run_tests.sh /run_tests.sh
RUN ["chmod", "+x", "/run_tests.sh"]

ENTRYPOINT ["/run_tests.sh"]

################################################################################
# Runtime stage: production runtime that serves the published app
################################################################################
FROM tirsvad/tirsvadcli_debian13_nginx:latest AS publish
SHELL ["/bin/bash"]
WORKDIR /workspace

# Copy the repo so tests can run without relying on a mounted workspace (optional)
COPY --from=builder /src /workspace

RUN ["mkdir", "/nuget"]

COPY tools/docker/publish.sh /publish.sh
RUN ["chmod", "+x", "/publish.sh"]

ENTRYPOINT ["bash", "/publish.sh"]

################################################################################
# Exporter stage: copies NuGet packages for host export
################################################################################
FROM publish AS exporter
COPY --from=publish /nuget /nuget
