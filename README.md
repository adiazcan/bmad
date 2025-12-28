# HR Agent - Conversational HR Assistant

A modern HR automation platform featuring a conversational AI agent for managing PTO requests, timesheets, and HR policy queries.

## Overview

HR Agent streamlines employee HR interactions through natural conversation, integrating with Factorial HR API and providing intelligent automation for common HR workflows.

**Tech Stack:**
- Backend: .NET 10 (ASP.NET Core Minimal APIs)
- Frontend: React 18+ with TypeScript, Vite 6+
- Infrastructure: Azure Container Apps, Cosmos DB, Blob Storage

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
  - Verify: `dotnet --version` should show 10.x.x
- **Node.js 20+** - [Download here](https://nodejs.org/)
  - Verify: `node --version` should show v20.x.x
- **Git** - For version control
- **Azure AD Tenant** (for authentication) - [Azure Portal](https://portal.azure.com)
  - Required for production authentication
  - Optional for local development (can use mock tokens in tests)

## Quick Start with .NET Aspire (Recommended)

.NET Aspire orchestrates both backend and frontend with a single command, providing an integrated dashboard for logs and telemetry.

### 1. Clone the Repository

```bash
git clone <repository-url>
cd bmad
```

### 2. Start Everything with Aspire

```bash
# Start AppHost - launches backend API + frontend + Aspire dashboard
cd HRAgent.AppHost
dotnet run --launch-profile http

# Or with HTTPS (dashboard):
dotnet run --launch-profile https
```

**What starts automatically:**
- ✅ Backend API on http://localhost:5000 (HTTPS varies)
- ✅ Frontend on http://localhost:5173
- ✅ Aspire Dashboard on http://localhost:15000

**Access the application:**
- Frontend: http://localhost:5173
- Backend API: http://localhost:5000/weatherforecast
- Aspire Dashboard: http://localhost:15000 (check logs, traces, metrics)

**Stop everything:** Press `Ctrl+C` in the terminal where AppHost is running.

### Benefits of Aspire Orchestration:
- 🚀 Single command starts all services
- 📊 Unified dashboard for logs and observability
- 🔗 Automatic service discovery (frontend knows backend URL)
- ⚡ Hot reload for both backend and frontend
- 🎯 Foundation for Azure Container Apps deployment

## Alternative: Manual Startup (Development)

## Alternative: Manual Startup (Development)

If you prefer to run services independently without Aspire:

### 1. Backend Setup

```bash
# Navigate to backend project
cd HRAgent.Api

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the API (starts on http://localhost:5000)
dotnet run
```

The backend API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:7187
- OpenAPI Spec: http://localhost:5000/openapi/v1.json (in Development mode)

> **Note:** By default, the "http" profile runs on port 5000 only. The "https" profile enables both HTTP (5000) and HTTPS (7187). You can specify the profile with `dotnet run --launch-profile http` or `dotnet run --launch-profile https`.

### 2. Frontend Setup

```bash
# Navigate to frontend project
cd ../hragent-ui

# Install dependencies (if not already installed)
npm install

# Set backend URL (create .env.local file)
echo "VITE_API_URL=http://localhost:5000" > .env.local

# Run development server (starts on http://localhost:5173)
npm run dev
```

The frontend will be available at http://localhost:5173 with Hot Module Replacement (HMR) enabled.

> **Note:** When using Aspire orchestration, the `VITE_API_URL` is set automatically. The `.env.local` file is only needed for standalone frontend development.

### 3. Build for Production

**Backend:**
```bash
cd HRAgent.Api
dotnet build --configuration Release
```

**Frontend:**
```bash
cd hragent-ui
npm run build
# Production files will be in dist/ folder
```

### 4. Run Tests

**Backend Tests:**
```bash
cd HRAgent.Api.Tests
dotnet test
# Runs 6 integration tests for weather endpoint
```

**Frontend Tests:**
```bash
cd hragent-ui
npm test
# Runs Vitest tests (use npm test -- --run for CI mode)
# Or run with UI: npm run test:ui
```

## Azure AD Authentication Setup

The backend API uses Microsoft.Identity.Web for Azure AD JWT token validation. Protected endpoints require valid JWT tokens.

### 1. Create Azure AD App Registration

**Using Azure Portal (portal.azure.com):**

1. Navigate to **Azure Active Directory** → **App registrations** → **New registration**
2. Configure the registration:
   - **Name**: `HRAgent API`
   - **Supported account types**: Single tenant (this directory only)
   - **Redirect URI**: Leave blank (not needed for API)
3. After creation, note the following values:
   - **Application (client) ID**: Copy this value
   - **Directory (tenant) ID**: Copy this value
4. Configure API scope:
   - Navigate to **Expose an API**
   - Add **Application ID URI**: `api://{client-id}` (replace {client-id} with your actual client ID)
   - Add a scope:
     - **Scope name**: `access_as_user`
     - **Who can consent**: Admins and users
     - **Display name**: Access HRAgent API
     - **Description**: Allows the app to access HRAgent API on behalf of the signed-in user

### 2. Configure appsettings.json

Update `HRAgent.Api/appsettings.json` with your Azure AD values:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "{your-tenant-id}",
    "ClientId": "{your-client-id}",
    "Audience": "api://{your-client-id}"
  }
}
```

For **local development**, you can use `appsettings.Development.json` with the "common" tenant for multi-tenant testing:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "common",
    "ClientId": "{your-dev-client-id}",
    "Audience": "api://{your-dev-client-id}"
  }
}
```

> **Security Note:** Never commit real credentials to Git. Add `appsettings.*.json` with secrets to `.gitignore`.

### 3. Test Authentication

**Without token (should return 401 Unauthorized):**
```bash
curl http://localhost:5000/secure
```

**With valid token (should return 200 OK):**
```bash
curl -H "Authorization: Bearer {your-jwt-token}" http://localhost:5000/secure
```

**Run integration tests:**
```bash
cd HRAgent.Api.Tests
dotnet test
# 6 authentication tests verify middleware behavior
```

### Authentication Flow

1. Frontend user initiates login via MSAL.js (Story 1.4 - coming soon)
2. User authenticates with Azure AD (username/password, MFA)
3. Azure AD issues JWT token with claims (userId, email, roles)
4. Frontend sends token in `Authorization: Bearer {token}` header
5. Backend validates token automatically via Microsoft.Identity.Web middleware
6. Protected endpoints extract userId from token claims

### Common Authentication Errors

**401 Unauthorized - Missing token:**
- Ensure `Authorization: Bearer {token}` header is present
- Check that the endpoint requires authorization (`.RequireAuthorization()`)

**401 Unauthorized - Invalid token:**
- Verify token is not expired
- Check token audience matches `Audience` in appsettings.json
- Ensure token issuer matches your Azure AD tenant

**CORS errors:**
- Frontend origin (http://localhost:5173) is configured in Program.cs CORS policy
- CORS middleware must be before Authentication middleware

## Project Structure

```
bmad/
├── HRAgent.AppHost/          # .NET Aspire orchestration (NEW in Story 1.2)
│   ├── Program.cs            # Service registration and configuration
│   ├── Properties/
│   │   └── launchSettings.json  # Launch profiles (http, https)
│   └── HRAgent.AppHost.csproj
├── HRAgent.Api/              # Backend API (.NET 10 Minimal APIs)
│   ├── Program.cs            # Main entry point + authentication middleware
│   ├── Properties/
│   │   └── launchSettings.json  # Port configuration (5000)
│   ├── appsettings.json      # Azure AD configuration (template)
│   ├── appsettings.Development.json  # Dev Azure AD configuration
│   └── HRAgent.Api.csproj
├── HRAgent.Api.Tests/        # Integration tests
│   ├── AuthenticationTests.cs  # JWT authentication tests (6 tests)
│   ├── AspireIntegrationTests.cs
│   └── HRAgent.Api.Tests.csproj
├── hragent-ui/               # Frontend SPA (React + TypeScript + Vite)
│   ├── src/
│   │   ├── main.tsx          # Entry point
│   │   ├── App.tsx           # Root component
│   │   ├── lib/
│   │   │   └── api-client.ts # API client with service discovery
│   │   └── ...
│   ├── .env.local            # Local env vars (fallback, not committed)
│   ├── package.json
│   ├── vite.config.ts
│   └── tsconfig.json
├── HRAgent.sln               # Solution file
└── README.md                 # This file
```

## Port Configuration

**With Aspire Orchestration (Recommended):**
- **Aspire Dashboard**: http://localhost:15000 (or https://localhost:15001)
- **Backend API**: Dynamically assigned (visible in Aspire dashboard)
- **Frontend**: Dynamically assigned (visible in Aspire dashboard)
- Frontend automatically receives backend URL via `VITE_API_URL` environment variable

**Manual Startup (Fallback):**
- **Backend API (HTTP)**: http://localhost:5000
- **Backend API (HTTPS)**: https://localhost:7187 (optional, use `--launch-profile https`)
- **Frontend Dev Server**: http://localhost:5173

## Aspire Dashboard Features

Access the dashboard at http://localhost:15000 to view:

- **Resources**: All running services (backend, frontend) with status indicators
- **Logs**: Aggregated console logs from all services with filtering
- **Traces**: Distributed tracing for request flows (OpenTelemetry)
- **Metrics**: Performance metrics (CPU, memory, request counts)
- **Environment**: View environment variables injected into each service

This provides complete observability during local development.

## Architecture Notes

### Backend - Minimal APIs Pattern

The backend uses **ASP.NET Core Minimal APIs** (NOT MVC Controllers):
- Endpoints are defined in `Program.cs` using `app.MapGet()`, `app.MapPost()`, etc.
- No `Controllers/` folder or `[ApiController]` attributes
- Endpoint-based routing for cleaner AG-UI protocol integration

### Frontend - Vite + React + TypeScript

The frontend uses modern tooling:
- **Vite 6+** for fast development and optimized production builds (NOT Create React App)
- **React 18+** with TypeScript strict mode
- Component-based architecture with PascalCase file naming

## Troubleshooting

### Aspire Issues

**Dashboard not loading:**
```bash
# Check that AppHost is running
# Dashboard URL is displayed in console output
# Default: http://localhost:15000
```

**Services not starting:**
```bash
# Check Aspire dashboard Resources tab for service status
# View logs in dashboard Logs tab for error details
# Ensure backend and frontend projects build successfully
```

**HTTPS certificate errors:**
```bash
# Use http profile which allows unsecured transport
dotnet run --launch-profile http
```

### Backend Issues

**Port already in use:**
```bash
# Change port in HRAgent.Api/Properties/launchSettings.json
# Update applicationUrl to use a different port
```

**Build errors:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

**Missing SDK:**
```bash
# Verify .NET 10 SDK is installed
dotnet --list-sdks
# Should show 10.x.x
```

### Frontend Issues

**Dependencies not installing:**
```bash
# Clear npm cache and reinstall
rm -rf node_modules package-lock.json
npm install
```

**Port 5173 already in use:**
```bash
# Vite will automatically use next available port (5174, 5175, etc.)
# Or specify port manually:
npm run dev -- --port 3000
```

**TypeScript errors:**
```bash
# Clear TypeScript cache
rm -rf node_modules/.cache
npm run build
```

## Development Workflow

**With Aspire (Recommended):**
1. **Start Everything**: `cd HRAgent.AppHost && dotnet run --launch-profile http`
2. **View Dashboard**: Open http://localhost:15000
3. **Make Changes**: Edit code with hot reload enabled for both services
4. **View Logs**: Check Aspire dashboard for real-time logs and traces
5. **Stop All**: Press `Ctrl+C` in AppHost terminal

**Manual Workflow:**
1. **Start Backend**: `cd HRAgent.Api && dotnet run`
2. **Start Frontend**: `cd hragent-ui && npm run dev`
3. **Make Changes**: Edit code with hot reload enabled
4. **Test**: Run tests before committing
5. **Build**: Run production builds before committing

## Azure Blob Storage Audit Logging

The backend implements immutable audit logging to Azure Blob Storage for legal compliance and transparency. All AI agent decisions and user actions are logged with complete context.

## MongoDB Database Configuration

The backend uses MongoDB for conversation state persistence with unified driver support for both local development and Azure DocumentDB production environments.

### Database Architecture

**Selected Option: Azure DocumentDB (Production) + MongoDB (Local)**

**Rationale:**
- **99.02% MongoDB Query Language compatibility** with Azure DocumentDB
- **Unified driver** - single `MongoDB.Driver` NuGet package for all environments
- **Local development excellence** - MongoDB Community Edition in Docker with feature parity
- **Production scaling** - Azure DocumentDB M200-Autoscale with instant capacity adjustment
- **Rich query language** - aggregation pipelines, LINQ integration, flexible document model

**Collections:**
- `conversations`: Conversation threads with messages (indexed by `threadId`)
- `user-patterns`: User behavior patterns for personalization (indexed by `userId`)

### Local Development Setup

**Aspire automatically manages MongoDB container** - no manual setup required:

```bash
# Start AppHost (includes MongoDB 7.0 container)
cd HRAgent.AppHost
dotnet run --launch-profile http

# Verify MongoDB in Aspire Dashboard
# Open http://localhost:15000 → Resources → "mongodb" should show Running
```

**Configuration:**
- Dev connection string in `appsettings.Development.json`: `mongodb://localhost:27017`
- Database name: `hragent-dev`
- Health check endpoint: `/ready` verifies MongoDB connectivity

### Test Endpoints (Development Only)

**Test MongoDB connectivity:**
```bash
curl http://localhost:5000/test-mongodb
# Response: { "success": true, "message": "MongoDB connected successfully", "threadId": "...", "messageCount": 1 }
```

**Query conversation by threadId:**
```bash
curl http://localhost:5000/conversations/{threadId}
# Response: { "id": "...", "threadId": "...", "userId": "...", "messages": [...] }
```

### Data Models

**ConversationThread:**
```csharp
{
  "id": "ObjectId",
  "threadId": "conv-abc123",
  "userId": "user@example.com",
  "createdAt": "2025-12-28T12:34:56.789Z",
  "updatedAt": "2025-12-28T13:00:00.000Z",
  "messages": [
    {
      "id": "msg-guid",
      "role": "user",
      "text": "Hello HRAgent",
      "createdAt": "2025-12-28T12:34:56.789Z",
      "tokenCount": 4
    }
  ]
}
```

**UserPattern:**
```csharp
{
  "id": "ObjectId",
  "userId": "user@example.com",
  "patternType": "timesheet",
  "patternData": "{\"frequency\":\"weekly\",\"dayOfWeek\":5}",
  "updatedAt": "2025-12-28T12:34:56.789Z"
}
```

### Repository Pattern

The backend implements repository pattern for data access:

**ConversationRepository:**
- `GetByThreadIdAsync(threadId)` - Retrieves conversation by threadId (uses index)
- `GetByUserIdAsync(userId)` - Retrieves all user conversations
- `AddAsync(thread)` - Inserts new conversation
- `UpdateAsync(thread)` - Upserts conversation (update or insert)
- `DeleteAsync(threadId)` - Deletes conversation

**PatternRepository:**
- `GetByUserIdAsync(userId)` - Retrieves user pattern (uses index)
- `UpsertAsync(pattern)` - Updates or inserts user pattern

### Integration Testing

MongoDB integration tests use **Testcontainers.MongoDb** for real MongoDB instances:

```bash
cd HRAgent.Api.Tests
dotnet test --filter "FullyQualifiedName~ConversationRepositoryTests|FullyQualifiedName~PatternRepositoryTests"
# 10 tests: CRUD operations, indexing, concurrency
```

**Tests verify:**
- ✅ CRUD operations (Create, Read, Update, Delete)
- ✅ Indexed queries (threadId, userId)
- ✅ Upsert behavior (update or insert)
- ✅ Null handling for non-existent documents
- ✅ Timestamp updates

### MongoDB Compass (Optional)

For visual inspection of local MongoDB data:

1. Install [MongoDB Compass](https://www.mongodb.com/products/compass)
2. Connect to: `mongodb://localhost:27017`
3. Select database: `hragent-dev`
4. Browse collections: `conversations`, `user-patterns`

### Production Configuration (Azure DocumentDB)

**Before deployment:**

1. **Create Azure DocumentDB cluster** (vCore-based, M200-Autoscale)
2. **Enable MongoDB API** with version 7.0+
3. **Configure firewall rules** for Azure Container Apps outbound IPs
4. **Store connection string in Azure Key Vault** as `MongoDbConnectionString`
5. **Update appsettings.json** with Key Vault reference (already templated)
6. **Enable managed identity** for Key Vault access

Production configuration template is already in place in `appsettings.json`:
```json
{
  "MongoDB": {
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/MongoDbConnectionString)",
    "DatabaseName": "hragent"
  }
}
```

**Production connection string format:**
```
mongodb+srv://<username>:<password>@<cluster>.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256
```

### Health Checks

The backend includes MongoDB health checks:

```bash
# Check all health checks
curl http://localhost:5000/health
# Response: Healthy (includes mongodb, storage, and other checks)

# Check readiness (mongodb + storage only)
curl http://localhost:5000/ready
# Response: Healthy (verifies MongoDB connectivity)
```

### Indexes

Indexes are automatically created on application startup:

- **conversations collection**: `threadId` (ascending) for fast conversation lookups
- **user-patterns collection**: `userId` (ascending) for fast pattern retrieval

Index creation is idempotent - safe to run multiple times.

## Azure Blob Storage Audit Logging

The backend implements immutable audit logging to Azure Blob Storage for legal compliance and transparency. All AI agent decisions and user actions are logged with complete context.

### Audit Logging Service

**Service:** `HRAgent.Api/Services/AuditLogger.cs`

**Features:**
- ✅ Thread-safe concurrent writes using `SemaphoreSlim`
- ✅ Retry logic with exponential backoff (3 attempts, doubling delay)
- ✅ JSON Lines format for append-only immutable logs
- ✅ Automatic blob path generation: `audit/{year}/{month}/{day}/{threadId}.jsonl`
- ✅ Complete audit entry capture: timestamp (UTC), userId, eventType, data, reasoning, threadId, correlationId, version

### Local Development Setup

**Aspire automatically manages Azurite emulator** - no manual setup required:

```bash
# Start AppHost (includes Azurite)
cd HRAgent.AppHost
dotnet run --launch-profile http

# Verify storage in Aspire Dashboard
# Open http://localhost:15000 → Resources → "storage" should show Running
```

**Configuration:**
- Dev connection string in `appsettings.Development.json`: `UseDevelopmentStorage=true`
- Container name: `audit-logs-dev`
- Health check endpoint: `/ready` verifies Blob Storage connectivity

### Test Endpoints (Development Only)

**Write test audit log:**
```bash
curl -X POST http://localhost:5000/test-audit
# Response: { "success": true, "message": "Audit log written successfully..." }
```

**Query audit logs by threadId:**
```bash
curl http://localhost:5000/test-audit/{threadId}
# Response: { "success": true, "threadId": "...", "logCount": 1, "logs": [...] }
```

### Integration Testing

A comprehensive integration test script is available:

```bash
# Make sure AppHost is running first
cd HRAgent.AppHost
dotnet run --launch-profile http

# In another terminal:
cd /home/adiaz/github/bmad
bash test-audit-integration.sh
```

**Tests performed:**
- ✅ Health check endpoint returns Healthy
- ✅ Audit log write succeeds
- ✅ Audit query returns logs
- ✅ Thread safety with 10 concurrent writes

### Azure Storage Explorer (Optional)

For visual inspection of audit logs:

1. Install [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)
2. Connect to local emulator: **Emulator - Default Ports (Key)**
3. Navigate to: **Blob Containers** → **audit-logs-dev**
4. Download `.jsonl` files to view entries

### Production Configuration

**Before deployment:**

1. **Create Azure Storage Account** (StorageV2, LRS, Cool tier)
2. **Configure 7-year immutability policy** on `audit-logs` container
3. **Store connection string in Azure Key Vault** as `BlobStorageConnectionString`
4. **Update appsettings.json** with Key Vault reference (already templated)
5. **Enable managed identity** for Key Vault access

Production configuration template is already in place in `appsettings.json`:
```json
{
  "BlobStorage": {
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://{vault}.vault.azure.net/secrets/BlobStorageConnectionString)",
    "ContainerName": "audit-logs"
  }
}
```

### Audit Log Schema

Each audit entry is a single JSON object per line (JSON Lines format):

```json
{
  "timestamp": "2025-12-28T12:34:56.789Z",
  "userId": "user@example.com",
  "eventType": "pto.request.submitted",
  "data": { "startDate": "2025-01-15", "endDate": "2025-01-20", "days": 4 },
  "reasoning": "User requested PTO for vacation. Policy allows 4 consecutive days. Balance sufficient.",
  "threadId": "conv-abc123def456",
  "correlationId": "req-789xyz",
  "version": "1.0"
}
```

**Key Fields:**
- `timestamp`: UTC timestamp (ISO 8601 with Z suffix)
- `userId`: From JWT token claims (Azure AD)
- `eventType`: Hierarchical event name (e.g., `pto.request.submitted`)
- `data`: Structured event data (JSON object)
- `reasoning`: AI agent decision reasoning or user action description
- `threadId`: Conversation thread ID (from Cosmos DB)
- `correlationId`: Request correlation ID for distributed tracing
- `version`: Schema version for backward compatibility

### Health Checks

The backend includes Blob Storage health checks:

```bash
# Check all health checks
curl http://localhost:5000/health
# Response: Healthy (includes storage, database, and other checks)

# Check readiness (storage + database only)
curl http://localhost:5000/ready
# Response: Healthy (verifies Blob Storage connectivity)
```

### Unit Tests

Run audit logger unit tests:

```bash
cd HRAgent.Api.Tests
dotnet test --filter "FullyQualifiedName~AuditLoggerTests"
# 5 tests: null validation, configuration validation, query functionality, field inclusion
```

## Next Steps

- ✅ **Story 1.1**: Initialize backend and frontend projects (COMPLETE)
- ✅ **Story 1.2**: Configure .NET Aspire orchestration (COMPLETE)
- ✅ **Story 1.3**: Set up Azure AD authentication - backend (COMPLETE)
- ✅ **Story 1.5**: Configure MongoDB/Azure DocumentDB connection (COMPLETE)
- ✅ **Story 1.6**: Configure Blob Storage for audit logs (COMPLETE)
- **Story 1.4**: Set up Azure AD authentication - frontend (MSAL.js)
- **Story 1.7**: Configure Factorial HR API client with Polly
- **Story 2.x**: Implement conversational interface with Microsoft Agent Framework

## Contributing

Please follow the coding standards defined in `_bmad-output/project-context.md`.

## License

[To be determined]

---

## BMAD Framework

This project uses BMAD (Business Model-Aware Development) for context engineering and AI-assisted development. BMAD configuration is located in `_bmad/`.

