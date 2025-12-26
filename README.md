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

## Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
cd bmad
```

### 2. Backend Setup

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

### 3. Frontend Setup

```bash
# Navigate to frontend project
cd ../hragent-ui

# Install dependencies (if not already installed)
npm install

# Run development server (starts on http://localhost:5173)
npm run dev
```

The frontend will be available at http://localhost:5173 with Hot Module Replacement (HMR) enabled.

### 4. Build for Production

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

### 5. Run Tests

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

## Project Structure

```
bmad/
├── HRAgent.Api/              # Backend API (.NET 10 Minimal APIs)
│   ├── Program.cs            # Main entry point with endpoint definitions
│   ├── Properties/
│   │   └── launchSettings.json  # Port configuration (5000)
│   ├── appsettings.json
│   └── HRAgent.Api.csproj
├── hragent-ui/               # Frontend SPA (React + TypeScript + Vite)
│   ├── src/
│   │   ├── main.tsx          # Entry point
│   │   ├── App.tsx           # Root component
│   │   └── ...
│   ├── package.json
│   ├── vite.config.ts
│   └── tsconfig.json
├── HRAgent.sln               # Solution file
└── README.md                 # This file
```

## Port Configuration

- **Backend API (HTTP)**: http://localhost:5000
- **Backend API (HTTPS)**: https://localhost:7187 (optional, use `--launch-profile https`)
- **Frontend Dev Server**: http://localhost:5173

These ports are configured for .NET Aspire orchestration (to be added in Story 1.2).

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

1. **Start Backend**: `cd HRAgent.Api && dotnet run`
2. **Start Frontend**: `cd hragent-ui && npm run dev`
3. **Make Changes**: Edit code with hot reload enabled
4. **Test**: Run tests (to be configured in later stories)
5. **Build**: Run production builds before committing

## Next Steps

- **Story 1.2**: Configure .NET Aspire orchestration for single-command startup
- **Story 1.3-1.4**: Add Azure AD authentication
- **Story 1.5-1.6**: Configure Cosmos DB and Blob Storage
- **Story 2.x**: Implement conversational interface with Microsoft Agent Framework

## Contributing

Please follow the coding standards defined in `_bmad-output/project-context.md`.

## License

[To be determined]

---

## BMAD Framework

This project uses BMAD (Business Model-Aware Development) for context engineering and AI-assisted development. BMAD configuration is located in `_bmad/`.

