# syntax=docker/dockerfile:1

# ---- Stage 1: build ----
# The SDK image has the compiler/tooling and is large (~800 MB+) - we only need it
# during build, not at runtime, which is the whole reason for a multi-stage build.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy just the .csproj first and restore - this layer is only invalidated when
# dependencies change, not on every source edit, so `docker build` reuses the
# cached restore on most rebuilds (see README "Layer caching").
COPY DockerDemo.csproj .
RUN dotnet restore

# Now copy the rest of the source and publish.
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# ---- Stage 2: runtime ----
# The ASP.NET runtime image has no SDK/compiler - much smaller (~200 MB) and a
# narrower attack surface, since nothing here can compile or run arbitrary dotnet CLI commands.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Run as a non-root user (the base image ships one) instead of the default root.
USER app

COPY --from=build /app .

# Purely documentation for humans/tools reading the Dockerfile - it does NOT
# actually publish the port. The real mapping happens with `docker run -p`.
EXPOSE 8080

# Docker restarts/reports the container unhealthy if this fails repeatedly -
# useful for orchestrators (docker compose, Kubernetes) to detect a stuck app.
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "DockerDemo.dll"]
