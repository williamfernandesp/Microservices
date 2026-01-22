# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj and restore first (layer caching)
COPY Fcg.Payment/src/Fcg.Payment.Web/Fcg.Payment.Web.csproj Fcg.Payment/src/Fcg.Payment.Web/
COPY Fcg.Payment/src/Fcg.Payment.Application/Fcg.Payment.Application.csproj Fcg.Payment/src/Fcg.Payment.Application/
COPY Fcg.Payment/src/Fcg.Payment.Common/Fcg.Payment.Common.csproj Fcg.Payment/src/Fcg.Payment.Common/
COPY Fcg.Payment/src/Fcg.Payment.Domain/Fcg.Payment.Domain.csproj Fcg.Payment/src/Fcg.Payment.Domain/
COPY Fcg.Payment/src/Fcg.Payment.Infra/Fcg.Payment.Infra.csproj Fcg.Payment/src/Fcg.Payment.Infra/
COPY Fcg.Payment/src/Fcg.Payment.Proxy.Auth/Fcg.Payment.Proxy.Auth.csproj Fcg.Payment/src/Fcg.Payment.Proxy.Auth/
COPY Fcg.Payment/src/Fcg.Payment.Proxy.Games/Fcg.Payment.Proxy.Games.csproj Fcg.Payment/src/Fcg.Payment.Proxy.Games/
COPY Fcg.Payment/src/Fcg.Payment.Proxy.User/Fcg.Payment.Proxy.User.csproj Fcg.Payment/src/Fcg.Payment.Proxy.User/
COPY Fcg.Payment/src/Fcg.Contracts/Fcg.Contracts.csproj Fcg.Payment/src/Fcg.Contracts/
COPY Fcg.Shared/Fcg.Observability/Fcg.Observability.csproj Fcg.Shared/Fcg.Observability/
RUN dotnet restore Fcg.Payment/src/Fcg.Payment.Web/Fcg.Payment.Web.csproj

# Copy everything and publish
COPY Fcg.Payment Fcg.Payment
COPY Fcg.Shared Fcg.Shared

# Clean obj folders that may have Windows paths and publish
RUN rm -rf Fcg.Payment/**/obj Fcg.Shared/**/obj && \
    dotnet publish Fcg.Payment/src/Fcg.Payment.Web/Fcg.Payment.Web.csproj -c Release -o /app/publish

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

ENTRYPOINT ["dotnet", "Fcg.Payment.Web.dll"]
