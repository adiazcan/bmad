---
stepsCompleted: [1, 2, 3, 4, 5]
inputDocuments: []
workflowType: 'research'
lastStep: 5
research_type: 'technical'
research_topic: 'Aspire 13 configuration with .NET 10 backend and React frontend'
research_goals: 'Comprehensive technical research covering Aspire 13 features, configuration patterns, React/Vite integration, troubleshooting common issues, and migration guidance'
user_name: 'Alberto'
date: '2025-12-27'
web_research_enabled: true
source_verification: true
completed: true
---

# Research Report: technical

**Date:** 2025-12-27
**Author:** Alberto
**Research Type:** technical

---

## Research Overview

This technical research document provides comprehensive analysis of .NET Aspire 13 configuration patterns for applications using .NET 10 backends and React frontends. The research covers architecture patterns, implementation approaches, integration strategies, and troubleshooting guidance based on current web sources and verified technical documentation.

---

## Technical Research Scope Confirmation

**Research Topic:** Aspire 13 configuration with .NET 10 backend and React frontend
**Research Goals:** Comprehensive technical research covering Aspire 13 features, configuration patterns, React/Vite integration, troubleshooting common issues, and migration guidance

**Technical Research Scope:**

- Architecture Analysis - design patterns, frameworks, system architecture
- Implementation Approaches - development methodologies, coding patterns
- Technology Stack - languages, frameworks, tools, platforms
- Integration Patterns - APIs, protocols, interoperability
- Performance Considerations - scalability, optimization, patterns

**Research Methodology:**

- Current web data with rigorous source verification
- Multi-source validation for critical technical claims
- Confidence level framework for uncertain information
- Comprehensive technical coverage with architecture-specific insights

**Scope Confirmed:** 2025-12-27

---

## Technology Stack Analysis

### Core Platform: .NET Aspire 13.0

**Aspire 13.0 Overview:**
.NET Aspire 13.0 represents a major transformation from ".NET Aspire" to simply "Aspire" - a full polyglot application platform. Released December 2025, Aspire 13.0 introduces first-class support for Python and JavaScript alongside .NET, comprehensive orchestration capabilities, and significant architectural improvements.

_Key Capabilities:_
- Cloud-native application orchestration and service discovery
- Built-in dashboard with observability (telemetry, traces, logs, metrics)
- Local development environment with hot reload support
- Container orchestration with automatic Docker/Podman integration
- Multiple deployment targets (Azure Container Apps, Kubernetes, Docker Compose)

_Source:_ https://aspire.dev/whats-new/aspire-13/

**Major Breaking Changes:**
Aspire 13.0 is a major version release with significant breaking changes. Most notably:
- Simplified SDK declaration: `Sdk="Aspire.AppHost.Sdk/13.0.0"` directly in the `<Project>` tag
- Automatic inclusion of `Aspire.Hosting.AppHost` package (no explicit reference needed)
- Target framework updated from `net9.0` to `net10.0`
- AddNpmApp() deprecated in favor of AddJavaScriptApp() and AddViteApp()
- Publishing infrastructure replaced with `aspire do` pipeline system

_Source:_ https://aspire.dev/whats-new/aspire-13/, https://learn.microsoft.com/en-us/dotnet/aspire/compatibility/13.0/

### Backend Framework: .NET 10 with Minimal APIs

**. NET 10 Integration:**
Aspire 13.0 requires .NET 10 SDK or later (starting with Preview 5+). This is a mandatory requirement as Aspire 13 specifically targets `net10.0` framework.

_Key .NET 10 Features for Aspire:_
- Enhanced ASP.NET Core Minimal APIs with improved endpoint-based routing
- C# 13 language features and performance improvements
- Advanced container optimization and trimming capabilities
- Improved OpenTelemetry integration for observability

_Requirements:_
- .NET 10 SDK Preview 5 or later (mandatory)
- OCI-compliant container runtime (Docker Desktop or Podman)
- Visual Studio 2022 17.9+, VS Code, or JetBrains Rider

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling, https://aspire.dev/whats-new/aspire-13/

**Project Structure:**
Aspire solutions use a standardized structure:
1. **AppHost Project** (`*.AppHost.csproj`): Orchestrator using `Aspire.AppHost.Sdk/13.0.0` SDK
2. **ServiceDefaults Project** (`*.ServiceDefaults.csproj`): Shared configuration for resilience, service discovery, and telemetry
3. **Service Projects**: Individual backend APIs (ASP.NET Core Minimal APIs)
4. **Frontend Projects**: React/Vite, Blazor, or other UI frameworks

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/aspire-sdk-templates

### Frontend Framework: React with Vite

**React Integration in Aspire 13.0:**
Aspire 13.0 introduces comprehensive JavaScript/React support with the new `AddViteApp` and `AddJavaScriptApp` APIs, replacing the deprecated `AddNpmApp` method.

_Key Features:_
- **AddViteApp()**: Specialized method for Vite-based React applications with optimizations
- **AddJavaScriptApp()**: Unified API for all JavaScript applications (npm, yarn, pnpm)
- Automatic package manager detection (npm, yarn, pnpm)
- Hot Module Replacement (HMR) support during development
- Automatic Dockerfile generation for production builds
- Service discovery integration with backend APIs

_Implementation Example:_
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add React/Vite frontend
var frontend = builder.AddViteApp("frontend", "./frontend")
    .WithReference(api);  // Automatic service discovery to backend

builder.Build().Run();
```

_Source:_ https://aspire.dev/whats-new/aspire-13/, https://learn.microsoft.com/en-us/dotnet/api/aspire.hosting.javascripthostingextensions.addviteapp

**Vite Configuration:**
- Default development script: `npm run dev`
- Default build script: `npm run build`
- Customizable with `WithRunScript()` and `WithBuildScript()`
- Automatic port binding configuration
- Node.js version auto-detection from `.nvmrc`, `.node-version`, `package.json`

_Package Manager Flexibility:_
```csharp
// Use yarn instead of npm
builder.AddViteApp("frontend", "./frontend")
    .WithYarn();

// Use pnpm
builder.AddViteApp("frontend", "./frontend")
    .WithPnpm();
```

_Source:_ https://aspire.dev/whats-new/aspire-13/

### Orchestration & Development Tools

**Aspire AppHost SDK:**
The `Aspire.AppHost.Sdk` provides orchestration capabilities:
- Project reference management with automatic code generation
- Service discovery between components
- Environment variable injection
- Container lifecycle management
- Health check configuration
- Dashboard integration

_Project File Structure (Aspire 13.0):_
```xml
<Project Sdk="Aspire.AppHost.Sdk/13.0.0">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\MyApp.Api\MyApp.Api.csproj" />
    <ProjectReference Include="..\MyApp.Web\MyApp.Web.csproj" />
  </ItemGroup>
</Project>
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dotnet-aspire-sdk, https://aspire.dev/whats-new/aspire-13/

**Aspire CLI:**
New command-line interface for Aspire 13.0:
- `aspire init`: Initialize Aspire orchestration in existing projects
- `aspire new`: Create projects from curated templates
- `aspire update`: Update Aspire packages (including `--self` for CLI updates)
- `aspire do`: Pipeline-based build, publish, and deployment system
- `aspire run`: Start the AppHost and all services
- `aspire add`: Add integrations to projects

_Source:_ https://aspire.dev/whats-new/aspire-13/

**VS Code Extension:**
Aspire 13.0 includes a new VS Code extension providing:
- Debug Python and C# projects inside VS Code
- Project creation from templates
- Integration management
- Launch configuration generation
- Publish and deployment commands (preview)
- Requires Aspire CLI on PATH

_Source:_ https://aspire.dev/whats-new/aspire-13/

### Container & Deployment Technologies

**Container Runtime:**
- Docker Desktop (recommended for Windows/macOS)
- Podman (Linux/cross-platform alternative)
- Automatic certificate trust for HTTPS in containers
- Multi-stage build support for production images

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling

**Deployment Targets:**
1. **Azure Container Apps** (primary target)
   - Automatic Azure resource provisioning via Bicep
   - Aspire Dashboard included by default in deployments
   - Application Insights integration available
   - Azure Developer CLI (`azd`) integration
   
2. **Docker Compose**
   - Local testing and lightweight deployments
   - Automatic compose file generation
   
3. **Kubernetes**
   - Manifest generation support
   - Custom resource definitions

_Source:_ https://aspire.dev/whats-new/aspire-13/, https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azd/aca-deployment

**Container Files as Build Artifacts:**
Aspire 13.0 introduces innovative build artifact handling:
```csharp
var frontend = builder.AddViteApp("frontend", "./frontend");
var api = builder.AddUvicornApp("api", "./api", "main:app");

// Extract frontend build output and copy to API container
api.PublishWithContainerFiles(frontend, "./static");
```

This enables patterns like building a React frontend in one container and serving it from a backend container.

_Source:_ https://aspire.dev/whats-new/aspire-13/

### Technology Adoption Trends

**Polyglot Platform Evolution:**
Aspire 13.0 marks a major shift from .NET-centric to truly polyglot:
- Python support: AddPythonApp, AddUvicornApp (FastAPI, Flask), automatic Dockerfile generation
- JavaScript support: Unified AddJavaScriptApp API, Vite/Node.js specializations
- Cross-language service discovery with simplified environment variables
- Polyglot connection properties (URI, JDBC formats for databases)

_Migration from Older Versions:_
- Applications using Aspire 8.x or 9.x must upgrade incrementally
- `AddNpmApp` users must migrate to `AddJavaScriptApp` or `AddViteApp`
- AppHost project files require SDK declaration updates
- Publishing callbacks replaced with pipeline steps

_Source:_ https://aspire.dev/whats-new/aspire-13/

**Development Workflow Improvements:**
- Single-file AppHost support for quick prototypes
- File-based C# applications (experimental)
- Improved non-interactive mode for CI/CD
- Deployment state management (remembers Azure configuration)
- Parallel execution in build/deployment pipelines

_Source:_ https://aspire.dev/whats-new/aspire-13/

**Community & Ecosystem:**
- New home at aspire.dev (replacing learn.microsoft.com/dotnet/aspire)
- Discord community: aka.ms/aspire-discord
- GitHub repository: dotnet/aspire
- MCP (Model Context Protocol) server in dashboard for AI assistant integration

_Source:_ https://aspire.dev/whats-new/aspire-13/

---

## Integration Patterns Analysis

### Service Discovery & Environment Variable Injection

**Aspire Service Discovery Mechanism:**
Aspire 13.0 provides automatic service discovery through the `WithReference()` API, which injects environment variables into consuming resources. This eliminates the need for hardcoded URLs or manual configuration.

_Environment Variable Formats:_

1. **Simplified Format** (Non-.NET applications - Python, JavaScript):
   - `{RESOURCE}_HTTP=http://api:8080`
   - `{RESOURCE}_HTTPS=https://api:8443`
   - Example: `API_HTTPS=https://localhost:7123`

2. **.NET Service Discovery Format**:
   - `services__{resourceName}__{endpointName}__{index}={uri}`
   - Example: `services__api__https__0=https://localhost:7123`
   - Used by .NET ServiceDiscovery client library

3. **Connection String Format** (Databases):
   - `ConnectionStrings__{resourceName}=connection-string`
   - Example: `ConnectionStrings__database=Host=postgres;...`

_Source:_ https://aspire.dev/whats-new/aspire-13/, https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/orchestrate-resources

**Implementation Example:**
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Backend API
var api = builder.AddProject<Projects.Api>("api");

// React frontend with automatic service discovery
var frontend = builder.AddViteApp("frontend", "./frontend")
    .WithReference(api);  // Injects API_HTTP and API_HTTPS env vars

// Named reference for multiple instances
var primary = builder.AddPostgres("db-primary").AddDatabase("users");
var replica = builder.AddPostgres("db-replica").AddDatabase("users");

builder.AddProject<Projects.Service>("service")
    .WithReference(primary, "primary")   // Custom name
    .WithReference(replica, "replica");  // Custom name
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/api/aspire.hosting.resourcebuilderextensions.withreference, https://aspire.dev/whats-new/aspire-13/

**Polyglot Connection Properties (New in Aspire 13.0):**
Database resources expose multiple connection string formats automatically:
- **URI Format**: `postgresql://user:pass@host:port/dbname`
- **JDBC Format**: `jdbc:postgresql://host:port/dbname`
- **Individual Properties**: `DB_HOST`, `DB_PORT`, `DB_USERNAME`, `DB_PASSWORD`

This enables any language to connect using their preferred format.

_Source:_ https://aspire.dev/whats-new/aspire-13/

### API Design & Communication Protocols

**HTTP/HTTPS Communication:**
Aspire enforces HTTPS by default for security. HTTP endpoints require explicit opt-in.

_HTTPS Enforcement:_
```csharp
// Error: Unsecured transport not allowed by default
// Set ASPIRE_ALLOW_UNSECURED_TRANSPORT=true to allow HTTP

// Best practice: Use HTTPS with external endpoints
var api = builder.AddProject<Projects.Api>("api")
    .WithExternalHttpEndpoints();  // Creates publicly accessible HTTPS endpoint
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/troubleshooting/allow-unsecure-transport

**External Service Integration:**
Aspire 13.0 provides `AddExternalService()` for third-party APIs:

```csharp
// Static URL
var nuget = builder.AddExternalService("nuget", "https://api.nuget.org/")
    .WithHttpHealthCheck(path: "/v3/index.json", statusCode: 200);

// Parameter-based (configurable)
var externalUrl = builder.AddParameter("external-api-url");
var external = builder.AddExternalService("external-api", externalUrl);

// Frontend references external service
var frontend = builder.AddViteApp("frontend", "./frontend")
    .WithReference(nuget);  // Service discovery for external API
```

_URL Requirements:_
- MUST be absolute URI (include scheme, host, optional port)
- MUST have path set to "/" (no additional segments)
- MUST NOT contain query parameters or fragments

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/orchestrate-resources

**Server-Sent Events (SSE) Support:**
Aspire supports SSE for real-time server-to-client communication. SSE is unidirectional (server pushes to client).

_Characteristics:_
- HTTP-based with `Content-Type: text/event-stream`
- Long-running connections
- Automatic retry logic on disconnection
- Browser support via EventSource API
- Preferred for live updates, dashboards, notifications

_Configuration Considerations:_
- Requires long-running connection support (4+ minute idle timeout)
- Disable response buffering for immediate event delivery
- Keep-alive messages recommended (`: keep-alive\n\n` every <4 min)
- Not suitable for Consumption tier services

_Source:_ https://learn.microsoft.com/en-us/azure/application-gateway/for-containers/server-sent-events, https://learn.microsoft.com/en-us/azure/api-management/how-to-server-sent-events

**WebSocket Support:**
Full-duplex, bidirectional communication over single TCP connection.

_Use Cases:_
- Chat applications
- Real-time collaboration
- Gaming
- Live streaming
- Continuous data feeds

_Note:_ Azure Front Door supports WebSocket without extra configuration. Aspire can orchestrate WebSocket endpoints through standard port binding.

_Source:_ https://learn.microsoft.com/en-us/azure/frontdoor/standard-premium/websocket

### Endpoint Configuration & Proxy Patterns

**Endpoint Reference System:**
Aspire 13.0 introduces network-aware endpoint resolution with `NetworkIdentifier`:

```csharp
var api = builder.AddProject<Projects.Api>("api");

// Get endpoint for specific network context
var localhostEndpoint = api.GetEndpoint("http", KnownNetworkIdentifiers.LocalhostNetwork);
var containerEndpoint = api.GetEndpoint("http", KnownNetworkIdentifiers.DefaultAspireContainerNetwork);

// Frontend uses localhost endpoint (browser access)
var frontend = builder.AddViteApp("frontend", "./frontend")
    .WithEnvironment("API_URL", api.GetEndpoint("https"));

// Worker uses container network endpoint (internal access)
var worker = builder.AddProject<Projects.Worker>("worker")
    .WithEnvironment("API_URL", 
        api.GetEndpoint("http", KnownNetworkIdentifiers.DefaultAspireContainerNetwork));
```

_Network Contexts:_
- **LocalhostNetwork**: Host machine access (e.g., `https://localhost:7123`)
- **DefaultAspireContainerNetwork**: Container-to-container (e.g., `http://api:8080`)
- **PublicInternet**: External internet access

_Source:_ https://aspire.dev/whats-new/aspire-13/

**Universal Container-to-Host Communication (Aspire 13.0):**
Major architectural improvement enabling reliable container-to-host connectivity:

```bash
# Enable experimental feature
export ASPIRE_ENABLE_CONTAINER_TUNNEL=true
```

This leverages DCP's container tunnel capability for seamless communication regardless of orchestrator support.

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/compatibility/13.0/

**Dev Tunnels Integration (Preview):**
Exposes localhost services to internet via Azure Dev Tunnels:

```csharp
var api = builder.AddProject<Projects.Api>("api");

// Create public dev tunnel
var publicDevTunnel = builder.AddDevTunnel("devtunnel-public")
    .WithAnonymousAccess()
    .WithReference(api.GetEndpoint("https"));

// Mobile app references dev tunnel
mauiApp.AddiOSSimulator()
    .WithReference(api, publicDevTunnel);
```

_Use Cases:_
- Mobile device testing (iOS/Android)
- External webhook callbacks
- Third-party service integration during development

_Environment Variables Injected:_
```
WEB_HTTPS=https://myweb-1234.westeurope.devtunnels.ms/
services__web__https__0=https://myweb-1234.westeurope.devtunnels.ms/
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/dev-tunnels-integration

### Observability & Telemetry Integration

**OpenTelemetry Built-in Support:**
Aspire 13.0 includes comprehensive OpenTelemetry integration for traces, metrics, and logs.

_Default Configuration:_
- Automatic instrumentation for ASP.NET Core, HttpClient, EF Core
- OTLP exporter to Aspire Dashboard
- Distributed tracing with Activity correlation
- Metrics collection with Meter API
- Structured logging with ILogger

_Implementation via ServiceDefaults:_
```csharp
// In ServiceDefaults project (auto-generated)
builder.ConfigureOpenTelemetry();  // Sets up OTel

// Configures:
// - Logging provider (ILogger → OTLP)
// - Metrics (ASP.NET, HttpClient, custom Meters)
// - Tracing (ASP.NET, HttpClient, custom ActivitySource)
// - OTLP exporter (to Dashboard)
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel, https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/telemetry

**Aspire Dashboard MCP Server:**
New in Aspire 13.0 - Model Context Protocol server for AI assistant integration:

_Capabilities:_
- `list_resources`: Query all resources, endpoints, environment variables
- `list_console_logs`: Access resource console output
- `list_structured_logs`: Retrieve telemetry logs filtered by resource
- `list_traces`: Access distributed trace information
- `execute_resource_command`: Execute commands on resources

_Authentication:_
- Streamable HTTP with API key (`x-mcp-api-key` header)
- Access from AI assistants (Claude, GitHub Copilot, Cursor, VS Code)

_Source:_ https://aspire.dev/whats-new/aspire-13/

**Telemetry Formats:**
- **OTLP (OpenTelemetry Protocol)**: Industry standard for telemetry export
- **Prometheus**: Metrics scraping endpoint
- **Jaeger**: Distributed tracing visualization
- **Application Insights**: Azure native APM

_Source:_ https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry

### Data Formats & Serialization

**Environment Variable Encoding:**
Aspire automatically handles environment variable encoding for complex data:

```csharp
// Spaces encoded in URLs
var api = builder.AddExternalService("my service", "https://my-service.com/");
// Results in: MY_SERVICE_HTTPS=https://my-service.com/

// Complex connection strings
var postgres = builder.AddPostgres("db").AddDatabase("mydb");
// Results in: ConnectionStrings__db=Host=postgres;Port=5432;Database=mydb;...
```

**Connection String Expressions:**
Resources expose connection string expressions for dynamic values:

```csharp
var redis = builder.AddRedis("cache");

builder.AddProject<Projects.Worker>("worker")
    .WithEnvironment("CACHE_CONNECTION", redis.Resource.ConnectionStringExpression);

// For identity-based connections (Azure)
builder.AddAzureFunctionsProject<Projects.Functions>("functions")
    .WithEnvironment("MyBinding" + 
        (builder.ExecutionContext.IsPublishMode ? "__serviceUri" : ""),
        resource.Resource.ConnectionStringExpression);
```

_Source:_ https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-aspire-integration

### Integration Security Patterns

**Certificate Trust Management:**
Aspire 13.0 automatically configures certificate trust across languages:

_Automatic Configuration:_
- **Python**: Sets `SSL_CERT_FILE` and `REQUESTS_CA_BUNDLE` environment variables
- **Node.js**: Sets `NODE_EXTRA_CA_CERTS` environment variable
- **Containers**: Mounts certificate bundles with proper environment variables
- **All Platforms**: Generates and manages development certificates

This enables HTTPS during local development without manual certificate configuration.

_Source:_ https://aspire.dev/whats-new/aspire-13/

**Health Checks Integration:**
External services support HTTP health checks for availability monitoring:

```csharp
var api = builder.AddExternalService("api", "https://api.example.com/")
    .WithHttpHealthCheck(path: "/health", statusCode: 200);
```

Health check status appears in Aspire Dashboard with "Running" or failure states.

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/orchestrate-resources

---

## Architectural Patterns and Design

### System Architecture Patterns

**Cloud-Native Microservices Architecture:**
Aspire 13.0 is purpose-built for cloud-native, microservices-based applications. This architecture provides significant advantages over traditional monolithic approaches.

_Microservices Characteristics:_
1. **Autonomous Development & Deployment**: Each service implements a specific business capability and can be developed/deployed independently
2. **Self-Contained**: Services encapsulate their own data storage, dependencies, and programming platform
3. **Process Isolation**: Each service runs in its own process (typically containerized)
4. **Standard Communication**: Services communicate via HTTP/HTTPS, gRPC, WebSockets, or AMQP
5. **Composable Applications**: Multiple microservices compose together to form the complete application

_Architecture Benefits:_
- **Agility**: Independent evolution and frequent deployment without full application redeployment
- **Granular Scaling**: Scale individual services based on demand rather than entire application
- **Technology Diversity**: Each service can use different technology stacks optimized for its needs
- **Fault Isolation**: Service failures don't cascade to entire system

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/definition, https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/microservices-architecture

**Aspire AppHost Orchestration Pattern:**
Aspire introduces a centralized orchestration model through the AppHost project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Backend services
var cache = builder.AddRedis("cache");
var db = builder.AddPostgres("postgres").AddDatabase("catalog");
var api = builder.AddProject<Projects.CatalogApi>("api")
    .WithReference(cache)
    .WithReference(db);

// Frontend
var frontend = builder.AddViteApp("webapp", "./webapp")
    .WithReference(api);

builder.Build().Run();
```

_Orchestration Responsibilities:_
- Service discovery and dependency injection
- Resource lifetime management (startup, shutdown, health monitoring)
- Network configuration and endpoint exposure
- Environment variable injection
- Development certificate management

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dotnet-aspire-sdk

**Container Orchestration & Clustering:**
Aspire leverages container orchestration for production-ready distributed systems:

_Key Capabilities:_
- **Automatic Load Balancing**: Distribute traffic across service instances
- **Service Scaling**: Handle multiple instances per service with automatic routing
- **Health Monitoring**: Track container health and restart failed instances
- **Resource Management**: Control CPU, memory allocation across containers
- **Rolling Updates**: Deploy new versions without downtime

_Orchestrator Support:_
- Development: Docker Compose via Aspire Dashboard
- Production: Kubernetes (AKS), Azure Container Apps
- Hybrid: Container-to-host communication with DCP tunnel

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/scalable-available-multi-container-microservice-applications

### Design Principles and Best Practices

**Clean Architecture & Domain-Driven Design (DDD):**
Aspire applications should follow established architectural principles for maintainability.

_Clean Architecture Layers (Concentric Circles):_
1. **Domain Core** (innermost): Entities, value objects, domain services, business rules
2. **Application Core**: Use cases, application services, interfaces/abstractions
3. **Infrastructure**: Data access, external services, framework dependencies
4. **Presentation**: UI, API controllers, endpoints

_Dependency Flow:_ Dependencies point inward only. Infrastructure and Presentation depend on Application Core, but Core never depends on outer layers.

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

**SOLID Principles in Aspire Microservices:**
SOLID principles are critical for maintainable microservice architectures:

1. **Single Responsibility**: Each service/class has one reason to change
2. **Open/Closed**: Open for extension, closed for modification
3. **Liskov Substitution**: Derived types must be substitutable for base types
4. **Interface Segregation**: Clients shouldn't depend on interfaces they don't use
5. **Dependency Inversion**: Depend on abstractions, not concretions

_Implementation with Dependency Injection:_
```csharp
// ASP.NET Core built-in IoC container
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Constructor injection (Explicit Dependencies Principle)
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    
    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }
}
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-web-api-design

**API Design Patterns:**
Aspire services expose well-defined APIs following REST and DDD principles:

_REST to DDD Mapping:_
- **Aggregates → Resources**: Domain aggregates map to REST resources (e.g., `/api/orders`)
- **Entities → Resource URLs**: Entity identities become resource URLs
- **Coarse-Grained Operations**: APIs expose aggregate-level operations, not internal state manipulation
- **HATEOAS**: Child entities reachable via links in parent representation

_API Versioning:_
- Maintain backward compatibility with versioned APIs
- Independent service updates without coordinating all consumers
- Use semantic versioning in URLs or headers

_Source:_ https://learn.microsoft.com/en-us/azure/architecture/microservices/design/api-design

**Domain-Driven Design (DDD) for Complex Services:**
Apply DDD patterns for microservices with significant business logic:

_When to Use DDD:_
- Complex business rules and domain logic
- Domain expert knowledge required
- Ever-changing business requirements
- Significant behavior beyond CRUD operations

_When NOT to Use DDD:_
- Simple CRUD services (over-engineering)
- Read-only data visualization
- Technical infrastructure services

_DDD Tactical Patterns:_
- **Entities**: Objects with unique identities
- **Value Objects**: Immutable objects defined by attributes
- **Aggregates**: Consistency boundaries grouping related entities
- **Repositories**: Data access abstraction
- **Domain Services**: Operations not belonging to entities
- **Factories**: Complex object creation encapsulation

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/, https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/develop-asp-net-core-mvc-apps

### Scalability and Performance Patterns

**Horizontal Scaling (Scale Out):**
Aspire services scale horizontally by adding more instances rather than upgrading resources.

_Scaling Strategies:_
- **Manual Scaling**: Fixed instance count configuration
- **Optimized Autoscale**: Automatic scaling based on CPU, cache utilization, ingestion metrics
- **Predictive Scaling**: Pattern recognition for seasonal demand (high confidence required)
- **Reactive Scaling**: Current usage-based scaling decisions

_Scaling Configuration Example:_
```csharp
// Azure Container Apps automatic scaling
var api = builder.AddProject<Projects.Api>("api")
    .WithReplicas(min: 2, max: 10);  // Scale between 2-10 instances
```

_Source:_ https://learn.microsoft.com/en-us/azure/data-explorer/manage-cluster-horizontal-scaling

**Distributed Caching for Performance:**
Redis distributed caching is the recommended approach for Aspire applications:

_Cache Patterns:_
1. **Data Cache (Cache-Aside)**: Load data from database only as needed, update cache on changes
2. **Content Cache**: Static content (headers, images, scripts) for faster page rendering
3. **Session Store**: User session data accessible across service instances
4. **Output Cache**: Complete HTML responses cached in Redis
5. **Job Queue**: Task queuing for long-running operations

_Implementation:_
```csharp
// AppHost configuration
var cache = builder.AddRedis("cache");

builder.AddProject<Projects.Api>("api")
    .WithReference(cache);

// Service configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = connectionString;
    options.InstanceName = "AspireApp";
});
```

_Performance Characteristics:_
- E10 tier: 1.4M GET/sec without SSL, 1.0M with SSL
- E100 tier: 3.0M GET/sec without SSL, 2.5M with SSL
- F1500 tier (scaled out): 1.6M GET/sec with capacity of 32

_Source:_ https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed, https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/cache-overview, https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/cache-best-practices-performance

**Load Balancing & Traffic Distribution:**
Distribute traffic across service instances for optimal performance:

_Load Balancing Strategies:_
- **Round-robin**: Equal distribution across instances
- **Least connections**: Direct to instance with fewest active connections
- **Geographic proximity**: Route to nearest instance (latency reduction)
- **Health-based**: Exclude unhealthy instances from rotation

_Azure Load Balancing Services:_
- **Azure Load Balancer**: Layer 4 (TCP/UDP) regional load balancing
- **Azure Application Gateway**: Layer 7 (HTTP/HTTPS) with WAF, SSL offload, URL routing
- **Azure Front Door**: Global layer 7 with CDN, DDoS protection, SSL offload

_Source:_ https://learn.microsoft.com/en-us/azure/networking/load-balancer-content-delivery/load-balancing-content-delivery-overview

**Database Scaling Patterns:**
Aspire microservices follow database-per-service pattern:

_Database Patterns:_
- **Database-per-Microservice**: Each service encapsulates its own database
- **Read Replicas**: Offload read operations to replica instances
- **Database Sharding**: Partition data across multiple database instances
- **CQRS (Command Query Responsibility Segregation)**: Separate read and write models

_NoSQL for High-Volume Services:_
- Sub-second response times required
- High scalability and resilience needs
- Document-oriented or key-value data models
- Examples: Azure Cosmos DB, MongoDB, Redis

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/distributed-data, https://learn.microsoft.com/en-us/azure/architecture/best-practices/caching

### Security Architecture Patterns

**Zero Trust Security Model:**
Aspire applications should implement Zero Trust principles:

_Three Principles:_
1. **Verify Explicitly**: Always authenticate and authorize based on all available data points
2. **Use Least Privilege Access**: Limit user/service access with Just-In-Time (JIT) and Just-Enough-Access (JEA)
3. **Assume Breach**: Minimize blast radius, segment access, verify end-to-end encryption

_Source:_ https://learn.microsoft.com/en-us/azure/key-vault/general/overview

**Secrets Management with Azure Key Vault:**
Never store secrets in code, configuration files, or environment variables in production.

_Key Vault Integration:_
```csharp
// AppHost configuration
var keyVault = builder.AddAzureKeyVault("secrets");

builder.AddProject<Projects.Api>("api")
    .WithReference(keyVault);

// Automatic configuration binding
var connectionString = configuration["ConnectionStrings:Database"];
```

_Key Vault Capabilities:_
- **Secrets Management**: API keys, passwords, connection strings
- **Key Management**: Encryption keys with HSM protection (FIPS 140-3 Level 3)
- **Certificate Management**: TLS/SSL certificates with automatic renewal
- **Access Control**: Azure RBAC integration for granular permissions
- **Audit Logging**: All access operations logged for compliance

_Security Best Practices:_
1. Use managed identities (no credentials in code)
2. Enable automatic secret rotation
3. Never use environment variables for production secrets
4. Implement secret expiration policies
5. Monitor access with Azure Monitor

_Source:_ https://learn.microsoft.com/en-us/azure/key-vault/general/overview, https://learn.microsoft.com/en-us/security/benchmark/azure/mcsb-v2-identity-management, https://learn.microsoft.com/en-us/aspnet/identity/overview/getting-started/introduction-to-aspnet-identity

**Identity & Access Management:**
Implement comprehensive identity strategy across all flows:

_Identity Types:_
1. **System-Managed Identities**: Azure resource access (Key Vault, storage, management plane)
2. **Workload Identities**: Service-to-service authentication within AKS clusters
3. **Managed Identities**: Component authentication without stored credentials
4. **Human Identities**: User authentication via Microsoft Entra ID (B2B, B2C, native)

_Authentication Standards:_
- OAuth 2.0 for authorization
- OpenID Connect for authentication
- JWT (JSON Web Tokens) for claims
- SAML for enterprise SSO

_Authorization Patterns:_
- Role-Based Access Control (RBAC)
- Attribute-Based Access Control (ABAC)
- Policy-Based Access Control (PBAC)
- Least Privilege Principle

_Source:_ https://learn.microsoft.com/en-us/azure/well-architected/security/identity-access, https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/landing-zone/design-area/identity-access-application-access

**Secure Communication Patterns:**
Enforce encryption and certificate management:

_TLS/SSL Enforcement:_
- HTTPS enforced by default in Aspire 13.0
- HTTP requires explicit `ASPIRE_ALLOW_UNSECURED_TRANSPORT=true` (development only)
- Automatic certificate trust configuration for Python, Node.js, containers
- Development certificates generated and managed automatically

_Mutual TLS (mTLS):_
- Client and server certificate verification
- Service-to-service authentication
- Zero Trust network architecture

_Source:_ https://learn.microsoft.com/en-us/azure/key-vault/certificates/secure-certificates

### Data Architecture Patterns

**Database-per-Microservice Pattern:**
Each microservice owns its data and exposes it only through its API.

_Pattern Benefits:_
- **Loose Coupling**: Services don't share databases, reducing dependencies
- **Technology Diversity**: Each service can use optimal database technology
- **Independent Scaling**: Scale databases independently based on service needs
- **Fault Isolation**: Database failures isolated to single service

_Challenges:_
- Distributed transactions (use Saga pattern)
- Data consistency across services (eventual consistency)
- Querying across service boundaries (API composition or CQRS)

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/distributed-data

**Polyglot Persistence:**
Aspire 13.0 embraces multiple database technologies within one application:

_Database Technology Selection:_
- **Relational (PostgreSQL, SQL Server)**: Transactional data, complex queries, ACID guarantees
- **Document (MongoDB, Cosmos DB)**: Semi-structured data, flexible schemas
- **Key-Value (Redis)**: Caching, session state, real-time data
- **Graph (Neo4j)**: Relationship-heavy data models
- **Time-Series (InfluxDB)**: IoT data, metrics, monitoring data

_Aspire Configuration:_
```csharp
var postgres = builder.AddPostgres("pg").AddDatabase("catalog");
var mongo = builder.AddMongoDB("mongo").AddDatabase("products");
var redis = builder.AddRedis("cache");

builder.AddProject<Projects.CatalogService>("catalog")
    .WithReference(postgres)
    .WithReference(redis);
    
builder.AddProject<Projects.ProductService>("products")
    .WithReference(mongo)
    .WithReference(redis);
```

_Source:_ https://aspire.dev/whats-new/aspire-13/

### Deployment and Operations Architecture

**Infrastructure as Code (IaC):**
Automate infrastructure provisioning for consistency and repeatability:

_IaC Tools:_
- **Azure Resource Manager (ARM)**: Native Azure templates
- **Terraform**: Multi-cloud infrastructure provisioning
- **Bicep**: Azure-specific DSL (simpler than ARM)
- **Azure CLI**: Scripted infrastructure commands

_Aspire Deployment Manifest:_
Aspire 13.0 generates deployment manifests automatically:

```bash
aspire do deploy  # Pipeline-based deployment
```

Generates JSON manifest describing all resources, dependencies, configuration for target environment.

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/summary, https://aspire.dev/whats-new/aspire-13/

**CI/CD Pipeline Architecture:**
Continuous integration and deployment mandatory for cloud-native applications:

_Pipeline Stages:_
1. **Build**: Compile, restore packages, run unit tests
2. **Test**: Integration tests, contract tests, security scans
3. **Package**: Create container images, tag with version
4. **Deploy**: Push to container registry, update orchestrator
5. **Verify**: Health checks, smoke tests, rollback on failure

_DevOps Platforms:_
- Azure DevOps Pipelines
- GitHub Actions
- GitLab CI/CD

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/summary

**Observability Architecture:**
Built-in observability is foundational to Aspire's design:

_Three Pillars:_
1. **Distributed Tracing**: Track requests across service boundaries with OpenTelemetry
2. **Metrics**: Performance counters, resource utilization, business metrics
3. **Structured Logging**: Centralized logs with correlation IDs

_Aspire Dashboard Integration:_
- Real-time visualization of traces, metrics, logs
- Resource monitoring (containers, endpoints, environment variables)
- Console output aggregation
- OTLP export to external APM tools (Application Insights, Jaeger, Prometheus)

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/telemetry, https://aspire.dev/whats-new/aspire-13/

---

## Implementation Approaches and Technology Adoption

### Technology Adoption Strategies

**Migration Path to Aspire 13.0:**
Aspire 13.0 is a major version release with significant breaking changes requiring careful planning.

_Automated Migration (Recommended):_
```bash
# Update Aspire CLI to latest version
curl -sSL https://aspire.dev/install.sh | bash  # Linux/macOS
# or
Invoke-RestMethod -Uri "https://aspire.dev/install.ps1" | Invoke-Expression  # Windows

# Automated upgrade of entire project
aspire update
```

The `aspire update` command automatically:
1. Updates `Aspire.AppHost.Sdk` version in AppHost project
2. Updates all Aspire NuGet packages to version 13.0
3. Handles dependency resolution automatically
4. Supports both regular projects and Central Package Management (CPM)

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/get-started/upgrade-to-aspire-13

**Manual Migration Steps (Alternative):**
For teams requiring granular control:

1. **Edit AppHost Project File** - Replace SDK declaration:
```xml
<!-- Before (9.x) -->
<Project Sdk="Microsoft.NET.Sdk">
  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
  </ItemGroup>
</Project>

<!-- After (13.0) -->
<Project Sdk="Aspire.AppHost.Sdk/13.0.0">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <!-- No explicit Aspire.Hosting.AppHost reference needed -->
</Project>
```

2. **Update NuGet Packages:**
```bash
dotnet add package Aspire.Hosting.Redis --version 13.0.0
dotnet add package Aspire.Hosting.PostgreSQL --version 13.0.0
# Repeat for all Aspire packages
```

3. **Address Breaking Changes:**
- Review complete breaking changes list: https://learn.microsoft.com/en-us/dotnet/aspire/compatibility/13.0/
- Update AddNpmApp() → AddViteApp() or AddJavaScriptApp()
- Change framework targets from net9.0 → net10.0
- Remove explicit Aspire.Hosting.AppHost references

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/get-started/upgrade-to-aspire-13

**Incremental Adoption Strategy:**
For existing monolithic applications:

_Phase 1: Proof of Concept (2-4 weeks)_
- Create new Aspire AppHost project
- Migrate 1-2 non-critical services
- Establish CI/CD pipeline
- Validate observability and service discovery

_Phase 2: Core Services (1-3 months)_
- Migrate authentication/authorization services
- Migrate database access layer
- Implement distributed caching
- Establish monitoring baselines

_Phase 3: Feature Services (3-6 months)_
- Migrate business logic services incrementally
- Implement event-driven patterns
- Optimize for cloud-native deployment

_Phase 4: Complete Migration (6-12 months)_
- Decommission monolithic components
- Implement full observability
- Optimize cost and performance

**Legacy System Modernization:**
- Use Strangler Fig Pattern: Incrementally replace monolith functionality
- Maintain backward compatibility during migration
- Run hybrid deployments (monolith + microservices)
- Monitor both systems during transition period

_Source:_ Microsoft Cloud Adoption Framework patterns

**Version Compatibility Matrix:**
| Aspire Version | .NET Version | Migration Path |
|----------------|--------------|----------------|
| 8.x (legacy) | .NET 8 | → 9.x → 13.0 (remove workload first) |
| 9.x | .NET 9 | → 13.0 (direct upgrade) |
| 13.0 | .NET 10 | Current stable |

_Important:_ Upgrading from Aspire 8.x requires first upgrading to 9.x, then to 13.0. Must remove legacy workload.

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/get-started/upgrade-to-aspire-13

### Development Workflows and Tooling

**Visual Studio Integration (17.9+):**
Comprehensive IDE support for Aspire development:

_Features:_
- Project templates for Aspire Starter Application, AppHost, ServiceDefaults
- **Add > .NET Aspire Orchestrator Support** context menu (enlist existing projects)
- Integrated debugging with Aspire Dashboard launch
- Azure deployment from Publish dialog
- GitHub Actions workflow generation
- Service dependency visualization

_Workflow:_
1. Create project: **File > New Project > Aspire Starter Application**
2. Run/Debug: Press F5 (launches AppHost + Aspire Dashboard)
3. Add services: Right-click project > **Add > .NET Aspire Orchestrator Support**
4. Deploy: Right-click AppHost > **Publish > Azure Container Apps**

_Source:_ https://learn.microsoft.com/en-us/visualstudio/azure/overview, https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling

**Visual Studio Code Integration:**
Requires C# Dev Kit extension + Aspire VS Code extension:

_Setup:_
```bash
# Install C# Dev Kit extension
code --install-extension ms-dotnettools.csdevkit

# Create project via Command Palette
Ctrl+Shift+P > "Create .NET Project" > Aspire Starter Application
```

_Key Features:_
- Configure launch.json for debugging (automatic setup)
- Run/Debug from editor title bar buttons
- **Aspire: Open Aspire terminal** command
- **Aspire: Publish deployment artifacts** (generates Bicep, Docker Compose, Helm charts)
- Integrated Aspire Dashboard browsing

_Debug Session:_
```bash
# From Aspire terminal
aspire run --start-debug-session
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/aspire-vscode-extension, https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling

**Aspire CLI Workflows:**
Cross-platform command-line tool for streamlined development:

_Core Commands:_
```bash
# Initialize new project
aspire init

# Create resources
aspire new apphost
aspire new service-defaults

# Run development environment
aspire run

# Update to latest version
aspire update --self
aspire update  # Update project dependencies

# Pipeline-based deployment
aspire do deploy

# Add integrations
aspire add redis
aspire add postgres
```

_Development Loop:_
1. Edit code in your IDE
2. `aspire run` (automatic rebuild + launch Dashboard)
3. Test changes in Dashboard
4. Commit changes to Git
5. `aspire do deploy` (CI/CD deployment)

_Source:_ https://aspire.dev/reference/cli/overview/, https://aspire.dev/whats-new/aspire-13/

**CI/CD Pipeline Integration:**

_GitHub Actions (Recommended):_
```yaml
name: Deploy Aspire to Azure
on:
  push:
    branches: [main]
  
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Install Azure Developer CLI
        run: curl -sSL https://azd.sh | bash
      
      - name: Login to Azure
        run: azd auth login --client-id ${{ secrets.AZURE_CLIENT_ID }}
      
      - name: Deploy to Azure
        run: azd deploy
        env:
          AZURE_SUBSCRIPTION_ID: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
```

_Azure DevOps Pipelines:_
```yaml
trigger:
  branches:
    include:
      - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '10.0.x'

- script: |
    curl -sSL https://azd.sh | bash
    azd deploy
  displayName: 'Deploy Aspire to Azure'
  env:
    AZURE_SUBSCRIPTION_ID: $(AZURE_SUBSCRIPTION_ID)
```

_Automated Configuration:_
```bash
# Azure Developer CLI automatically configures CI/CD
azd pipeline config
```

This command:
1. Creates GitHub repository (if needed)
2. Sets up authentication secrets
3. Generates workflow files
4. Configures Azure service connections

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azd/aca-deployment-github-actions, https://learn.microsoft.com/en-us/visualstudio/azure/overview-github-actions

### Testing and Quality Assurance

**Unit Testing with xUnit (Recommended):**
Test individual service components in isolation:

```csharp
public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_ValidInput_ReturnsOrderId()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(new Order { Id = 123 });
        
        var service = new OrderService(mockRepo.Object);
        
        // Act
        var orderId = await service.CreateOrderAsync(new OrderRequest());
        
        // Assert
        Assert.Equal(123, orderId);
    }
}
```

_Test Frameworks:_
- **xUnit.net** (recommended): Modern, extensible, community-driven
- **NUnit**: Feature-rich, JUnit port with wide adoption
- **MSTest**: Microsoft's built-in testing framework

_Source:_ https://learn.microsoft.com/en-us/dotnet/core/testing/, https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps

**Integration Testing with WebApplicationFactory:**
Test service interactions including infrastructure:

```csharp
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace database with in-memory version
                    services.RemoveAll<DbContext>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            })
            .CreateClient();
    }
    
    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotEmpty(products);
    }
}
```

_Integration Test Scope:_
- Database interactions (use in-memory or test databases)
- API endpoints (request/response validation)
- Authentication/authorization flows
- Service-to-service communication
- Caching behavior

_Source:_ https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests

**Multi-Container Service Testing:**
Test complete Aspire application with all dependencies:

```bash
# docker-compose.test.yml
services:
  api-tests:
    build:
      context: .
      dockerfile: Tests.Dockerfile
    depends_on:
      - api
      - postgres
      - redis
    environment:
      ConnectionStrings__Database: "Host=postgres;Database=testdb;..."
      ConnectionStrings__Cache: "redis:6379"
    command: dotnet test --logger "console;verbosity=detailed"
```

_Service Test Strategy:_
1. Start all services with `docker-compose up`
2. Run end-to-end tests against live services
3. Verify service discovery, communication, data persistence
4. Tear down environment after tests

_Source:_ https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps

**Testing Best Practices:**
- **Arrange-Act-Assert Pattern**: Structure all tests consistently
- **Test Isolation**: Each test independent, no shared state
- **Fast Execution**: Unit tests < 100ms, integration tests < 5 seconds
- **Meaningful Names**: `MethodName_Scenario_ExpectedBehavior`
- **Code Coverage**: Aim for 80%+ critical path coverage
- **Continuous Testing**: Run tests in CI/CD pipeline on every commit

_Source:_ https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices

### Deployment and Operations Practices

**Azure Container Apps Deployment (Recommended):**
Fully managed serverless container platform optimized for Aspire:

_One-Command Deployment:_
```bash
# Install Azure Developer CLI
curl -sSL https://azd.sh | bash

# Initialize Aspire project for Azure
azd init

# Provision + Deploy in single command
azd up
```

_What `azd up` Does:_
1. **Provision Infrastructure**: Resource group, Container Registry, Container Apps Environment, Redis, PostgreSQL
2. **Build Container Images**: Multi-stage Docker builds for each service
3. **Push to Registry**: Uploads images to Azure Container Registry
4. **Deploy Services**: Creates Container Apps with proper configuration
5. **Configure Networking**: Sets up ingress, service discovery, environment variables
6. **Enable Monitoring**: Application Insights integration

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azd/aca-deployment

**Deployment Options Comparison:**

| Deployment Target | Complexity | Cost | Scalability | Use Case |
|-------------------|------------|------|-------------|----------|
| **Azure Container Apps** | Low | Low-Medium | Excellent | Production (recommended) |
| **Azure Kubernetes Service (AKS)** | High | Medium-High | Excellent | Enterprise, advanced orchestration |
| **Docker Compose** | Very Low | Minimal | Limited | Development, testing |
| **Kubernetes (self-managed)** | Very High | Variable | Excellent | Maximum control required |

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/deployment/overview

**Infrastructure as Code with Bicep:**
Generate infrastructure templates from Aspire manifest:

```bash
# Generate Bicep files from Aspire project
aspire do publish --output ./infrastructure
```

Generated files:
- `main.bicep`: Main infrastructure template
- `*.bicep`: Individual resource modules
- `parameters.json`: Environment-specific configuration

_Customize and Deploy:_
```bash
# Deploy with Azure CLI
az deployment group create \
  --resource-group myResourceGroup \
  --template-file main.bicep \
  --parameters parameters.json
```

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment

**Monitoring and Observability:**
Built-in Application Insights integration:

```csharp
// Automatic telemetry collection
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ApiService>("api")
    .WithEnvironment("APPLICATIONINSIGHTS_CONNECTION_STRING", 
        builder.Configuration["ApplicationInsights:ConnectionString"]);
```

_Observability Features:_
- Distributed tracing across services
- Performance metrics (request duration, throughput)
- Log aggregation with correlation IDs
- Live metrics streaming
- Dependency tracking
- Failure rate monitoring

_Aspire Dashboard (Development):_
- Real-time trace visualization
- Console log aggregation
- Resource health monitoring
- Environment variable inspection

_Source:_ https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/telemetry

**Operations Best Practices:**
1. **Health Checks**: Implement `/health` endpoints for all services
2. **Graceful Shutdown**: Handle SIGTERM for clean container stops
3. **Resource Limits**: Set CPU/memory limits to prevent runaway processes
4. **Auto-Scaling Rules**: Configure based on CPU, memory, HTTP queue length
5. **Blue-Green Deployments**: Zero-downtime updates using Container Apps revisions
6. **Rollback Strategy**: Maintain previous revision for instant rollback

### Cost Optimization and Resource Management

**Azure Container Apps Cost Model:**
Pay only for resources consumed:

_Consumption Plan (Scale-to-Zero):_
- **Idle Cost**: $0 (no minimum charge)
- **Active Cost**: $0.000012/vCPU-second + $0.000001/GiB-second
- **Best For**: Variable workloads, development environments

_Dedicated Plan (Reserved Capacity):_
- **Fixed Cost**: Based on node count (minimum 3 nodes)
- **Per Node**: $0.184/hour (D4 instance)
- **Best For**: Predictable workloads, enterprise applications

_Example Cost Calculation (Consumption):_
```
API Service: 0.25 vCPU, 0.5 GiB RAM, 10M requests/month
- CPU: 0.25 * 2,592,000 seconds * $0.000012 = $7.78
- Memory: 0.5 * 2,592,000 seconds * $0.000001 = $1.30
- Total: ~$9/month
```

_Cost Drivers (Typical Aspire App):_
1. **Compute**: Container Apps execution time
2. **Storage**: Azure Container Registry, persistent volumes
3. **Data Services**: Cosmos DB, Azure SQL, Redis
4. **Networking**: Egress traffic (free within region)

_Source:_ https://azure.microsoft.com/pricing/details/container-apps/, https://learn.microsoft.com/en-us/azure/well-architected/service-guides/azure-container-apps

**Cost Optimization Strategies:**

_Right-Sizing Resources:_
```csharp
// Configure resource limits
var api = builder.AddProject<Projects.Api>("api")
    .WithReplicas(min: 2, max: 10)  // Auto-scale between 2-10
    .WithResourceLimits(cpu: 0.5, memory: 1.0);  // Optimize allocation
```

_Scale-to-Zero for Idle Services:_
```csharp
// Enable scale-to-zero for background jobs
var worker = builder.AddProject<Projects.Worker>("worker")
    .WithReplicas(min: 0, max: 5)  // Scale to zero when idle
    .WithHttpHealthCheck("/health", statusCode: 200);
```

_Regional Co-location:_
- Deploy all services in same region (avoid cross-region charges)
- Use Azure Front Door for global distribution
- Replicate data stores regionally only when required

_Reserved Capacity (Long-term Workloads):_
- Azure Savings Plan: Up to 17% discount (1-3 year commitment)
- Reserved Instances: Predictable workloads benefit from fixed pricing

_Source:_ https://learn.microsoft.com/en-us/azure/well-architected/cost-optimization/optimize-scaling-costs

**Resource Management Best Practices:**
1. **Monitor Utilization**: Use Azure Monitor to track CPU, memory, request rates
2. **Set Budgets**: Azure Cost Management alerts when approaching limits
3. **Tag Resources**: Consistent tagging for cost allocation and reporting
4. **Review Regularly**: Monthly cost reviews to identify optimization opportunities
5. **Use Managed Services**: Leverage managed Redis, PostgreSQL to reduce operational overhead

### Risk Assessment and Mitigation

**Common Migration Risks:**

| Risk | Impact | Mitigation Strategy |
|------|--------|---------------------|
| **Breaking Changes** | High | Automated testing, gradual rollout, maintain rollback capability |
| **Performance Degradation** | Medium | Load testing, performance baselines, monitoring |
| **Data Loss** | Critical | Database backups, transaction logging, disaster recovery plan |
| **Service Downtime** | High | Blue-green deployment, health checks, circuit breakers |
| **Cost Overruns** | Medium | Budget alerts, resource limits, regular cost reviews |
| **Security Vulnerabilities** | Critical | Secrets management, vulnerability scanning, least privilege access |

**Technical Debt Management:**
- Document architectural decisions (ADRs)
- Regular refactoring sprints
- Automated code quality checks (SonarQube, CodeQL)
- Dependency updates (Dependabot, Renovate)

**Disaster Recovery Planning:**
- **RTO (Recovery Time Objective)**: < 1 hour
- **RPO (Recovery Point Objective)**: < 15 minutes
- **Backup Strategy**: Automated daily backups with point-in-time restore
- **Failover Testing**: Quarterly DR drills

---

## Technical Research Recommendations

### Implementation Roadmap

**Phase 1: Foundation (Weeks 1-4)**
1. Install .NET 10 SDK and Aspire tooling
2. Create proof-of-concept Aspire application
3. Establish CI/CD pipeline with GitHub Actions or Azure DevOps
4. Deploy to Azure Container Apps (development environment)
5. Validate observability with Aspire Dashboard and Application Insights

**Phase 2: Core Services Migration (Months 2-3)**
1. Identify 2-3 core services for migration
2. Implement service-to-service communication patterns
3. Configure distributed caching with Redis
4. Establish database-per-service architecture
5. Implement comprehensive testing strategy

**Phase 3: Production Readiness (Months 4-6)**
1. Security hardening (Azure Key Vault, managed identities)
2. Performance optimization (load testing, tuning)
3. Cost optimization (right-sizing, scale-to-zero)
4. Production deployment with blue-green strategy
5. Runbook creation for operations team

**Phase 4: Full Migration (Months 7-12)**
1. Migrate remaining services incrementally
2. Decommission legacy monolithic components
3. Optimize based on production metrics
4. Continuous improvement iterations

### Technology Stack Recommendations

**Aspire 13.0 Configuration for HRAgent:**
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Backend API (.NET 10)
var api = builder.AddProject<Projects.HRAgentApi>("api")
    .WithReplicas(min: 2, max: 10);

// React Frontend (Vite)
var frontend = builder.AddViteApp("webapp", "./hragent-ui")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("https"));

builder.Build().Run();
```

**Recommended Services:**
- **Backend**: ASP.NET Core 10.0 with Minimal APIs
- **Frontend**: React 18+ with Vite 5+
- **Cache**: Azure Cache for Redis (E10+ tier)
- **Database**: Azure PostgreSQL Flexible Server
- **Monitoring**: Application Insights with OpenTelemetry
- **Deployment**: Azure Container Apps (Consumption plan initially)

### Skill Development Requirements

**Team Training Priorities:**
1. **.NET 10 & Aspire Fundamentals** (1 week)
   - Aspire architecture and orchestration
   - Service discovery and communication patterns
   - Aspire Dashboard usage

2. **Microservices Architecture** (2 weeks)
   - Domain-driven design principles
   - Event-driven patterns
   - Distributed system challenges

3. **Cloud-Native Development** (2 weeks)
   - Container fundamentals
   - Azure Container Apps
   - CI/CD with GitHub Actions

4. **Observability & Operations** (1 week)
   - OpenTelemetry integration
   - Application Insights
   - Production troubleshooting

**Recommended Learning Resources:**
- Microsoft Learn: [.NET Aspire Learning Path](https://learn.microsoft.com/training/paths/dotnet-aspire/)
- eBook: [Architecting Cloud Native .NET Applications for Azure](https://learn.microsoft.com/dotnet/architecture/cloud-native/)
- Hands-on Lab: [Aspire Workshop](https://github.com/dotnet-presentations/aspire-workshop)

### Success Metrics and KPIs

**Technical Metrics:**
- **Deployment Frequency**: Target daily deployments
- **Lead Time for Changes**: < 1 day from commit to production
- **Mean Time to Recovery (MTTR)**: < 30 minutes
- **Change Failure Rate**: < 15%
- **Service Availability**: 99.9% uptime (SLA)

**Performance Metrics:**
- **API Response Time**: p95 < 200ms
- **Error Rate**: < 0.1% of requests
- **Resource Utilization**: 60-80% average CPU/memory
- **Cache Hit Rate**: > 90%

**Business Metrics:**
- **Feature Delivery Velocity**: 20% increase quarter-over-quarter
- **Infrastructure Cost**: < 5% of revenue
- **Developer Productivity**: 30% reduction in time-to-market
- **System Reliability**: Zero unplanned downtime

---

**🎉 Technical Research Complete!**

This comprehensive research covers:
✅ Technology stack analysis (Aspire 13.0, .NET 10, React/Vite)
✅ Integration patterns (service discovery, communication, OpenTelemetry)
✅ Architectural patterns (microservices, DDD, security, scalability)
✅ Implementation approaches (migration, workflows, testing, deployment)
✅ Practical recommendations and roadmap

**Next Steps for HRAgent AppHost Issue:**
Based on this research, your Exit Code 134 crash likely stems from:
1. Incorrect SDK declaration in AppHost project file
2. Framework target mismatch (needs net10.0)
3. Breaking changes from deprecated AddNpmApp() method

**Recommended Action:**
Inspect `HRAgent.AppHost.csproj` and verify it follows Aspire 13.0 format with `Sdk="Aspire.AppHost.Sdk/13.0.0"` and `<TargetFramework>net10.0</TargetFramework>`.

<!-- Content will be appended sequentially through research workflow steps -->
