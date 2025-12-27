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

## Next Steps

- ✅ **Story 1.1**: Initialize backend and frontend projects (COMPLETE)
- ✅ **Story 1.2**: Configure .NET Aspire orchestration (COMPLETE)
- ✅ **Story 1.3**: Set up Azure AD authentication - backend (COMPLETE)
- **Story 1.4**: Set up Azure AD authentication - frontend (MSAL.js)
- **Story 1.5-1.6**: Configure Cosmos DB and Blob Storage
- **Story 2.x**: Implement conversational interface with Microsoft Agent Framework

## Contributing

Please follow the coding standards defined in `_bmad-output/project-context.md`.

## License

[To be determined]

---

## BMAD Framework

This project uses BMAD (Business Model-Aware Development) for context engineering and AI-assisted development. BMAD configuration is located in `_bmad/`.

