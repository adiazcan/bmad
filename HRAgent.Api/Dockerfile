# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (layer caching)
COPY ["HRAgent.Api/HRAgent.Api.csproj", "HRAgent.Api/"]
RUN dotnet restore "HRAgent.Api/HRAgent.Api.csproj"

# Copy source and build
COPY HRAgent.Api/ HRAgent.Api/
WORKDIR "/src/HRAgent.Api"
RUN dotnet build "HRAgent.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HRAgent.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Install wget for health checks
RUN apk add --no-cache wget

# Security: Create non-root user
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -s /bin/sh -D appuser

# Copy published app
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appgroup /app
USER appuser

# Expose port 8080 (Container Apps ingress target)
EXPOSE 8080

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Start application
ENTRYPOINT ["dotnet", "HRAgent.Api.dll"]
