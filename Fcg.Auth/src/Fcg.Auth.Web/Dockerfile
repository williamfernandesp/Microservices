# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj and restore first (layer caching)
COPY Fcg.Auth/src/Fcg.Auth.Web/Fcg.Auth.Web.csproj Fcg.Auth/src/Fcg.Auth.Web/
COPY Fcg.Auth/src/Fcg.Auth.Application/Fcg.Auth.Application.csproj Fcg.Auth/src/Fcg.Auth.Application/
COPY Fcg.Auth/src/Fcg.Auth.Domain/Fcg.Auth.Domain.csproj Fcg.Auth/src/Fcg.Auth.Domain/
COPY Fcg.Auth/src/Fcg.Auth.Infra/Fcg.Auth.Infra.csproj Fcg.Auth/src/Fcg.Auth.Infra/
COPY Fcg.Auth/src/Fcg.Auth.Common/Fcg.Auth.Common.csproj Fcg.Auth/src/Fcg.Auth.Common/
COPY Fcg.Auth/src/Fcg.Auth.Proxy/Fcg.Auth.Proxy.User.csproj Fcg.Auth/src/Fcg.Auth.Proxy/
COPY Fcg.Shared/Fcg.Observability/Fcg.Observability.csproj Fcg.Shared/Fcg.Observability/
RUN dotnet restore Fcg.Auth/src/Fcg.Auth.Web/Fcg.Auth.Web.csproj

# Copy everything and publish
COPY Fcg.Auth Fcg.Auth
COPY Fcg.Shared Fcg.Shared

# Clean obj folders that may have Windows paths and publish
RUN rm -rf Fcg.Auth/**/obj Fcg.Shared/**/obj && \
    dotnet publish Fcg.Auth/src/Fcg.Auth.Web/Fcg.Auth.Web.csproj -c Release -o /app/publish

# Runtime stage - Alpine for smaller image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Install required packages for globalization
RUN apk add --no-cache icu-libs tzdata

# Create non-root user for security
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D -s /bin/sh appuser

WORKDIR /app

# Copy published files
COPY --from=build --chown=appuser:appgroup /app/publish .

# Switch to non-root user
USER appuser

# Expose port and configure environment
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "Fcg.Auth.Web.dll"]
