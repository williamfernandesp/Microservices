# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj and restore first (layer caching)
COPY Fcg.User/src/Fcg.User.Web/Fcg.User.Web.csproj Fcg.User/src/Fcg.User.Web/
COPY Fcg.User/src/Fcg.User.Application/Fcg.User.Application.csproj Fcg.User/src/Fcg.User.Application/
COPY Fcg.User/src/Fcg.User.Domain/Fcg.User.Domain.csproj Fcg.User/src/Fcg.User.Domain/
COPY Fcg.User/src/Fcg.User.Common/Fcg.User.Common.csproj Fcg.User/src/Fcg.User.Common/
COPY Fcg.User/src/Fcg.User.Infra/Fcg.User.Infra.csproj Fcg.User/src/Fcg.User.Infra/
COPY Fcg.User/src/Fcg.User.Proxy.Auth/Fcg.User.Proxy.Auth.csproj Fcg.User/src/Fcg.User.Proxy.Auth/
COPY Fcg.User/src/Fcg.User.Proxy.Games/Fcg.User.Proxy.Games.csproj Fcg.User/src/Fcg.User.Proxy.Games/
COPY Fcg.Payment/src/Fcg.Contracts/Fcg.Contracts.csproj Fcg.Payment/src/Fcg.Contracts/
COPY Fcg.Shared/Fcg.Observability/Fcg.Observability.csproj Fcg.Shared/Fcg.Observability/
RUN dotnet restore Fcg.User/src/Fcg.User.Web/Fcg.User.Web.csproj

# Copy everything and publish
COPY Fcg.User Fcg.User
COPY Fcg.Payment Fcg.Payment
COPY Fcg.Shared Fcg.Shared

# Clean obj folders that may have Windows paths and publish
RUN rm -rf Fcg.User/**/obj Fcg.Payment/**/obj Fcg.Shared/**/obj && \
    dotnet publish Fcg.User/src/Fcg.User.Web/Fcg.User.Web.csproj -c Release -o /app/publish

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

ENTRYPOINT ["dotnet", "Fcg.User.Web.dll"]
