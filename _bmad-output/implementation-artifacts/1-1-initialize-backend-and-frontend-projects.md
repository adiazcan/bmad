# Story 1.1: Initialize Backend and Frontend Projects

**Status:** ready-for-dev  
**Epic:** 1 - Project Foundation & Development Environment  
**Story ID:** 1.1  
**Created:** 2025-12-26

---

## Story

As a **developer**,  
I want to **initialize the backend API and frontend SPA projects using official starter templates**,  
So that **the team has a working foundation to build upon with proper tooling and structure**.

---

## Acceptance Criteria

**Given** the project repository is empty  
**When** I run the initialization commands  
**Then:**

1. ✅ Backend project is created using `dotnet new webapi -n HRAgent.Api -f net10.0`
2. ✅ Frontend project is created using Vite React TypeScript template
3. ✅ Both projects have proper .gitignore files
4. ✅ README.md includes setup instructions for local development
5. ✅ Projects can be built successfully:
   - Backend: `dotnet build` completes without errors
   - Frontend: `npm install` and `npm run build` complete without errors
6. ✅ Backend runs on http://localhost:5000 with minimal API endpoint
7. ✅ Frontend runs on http://localhost:5173 with React dev server
8. ✅ Solution file (HRAgent.sln) includes backend project

---

## Tasks / Subtasks

### Task 1: Initialize Backend Project (AC: 1, 5, 6, 8)
- [ ] Create backend directory and navigate to it
- [ ] Run `dotnet new webapi -n HRAgent.Api -f net10.0`
- [ ] Verify Program.cs contains Minimal APIs pattern (no Controllers)
- [ ] Add .gitignore for .NET projects
- [ ] Test build: `dotnet build` succeeds
- [ ] Test run: `dotnet run` starts API on port 5000
- [ ] Verify default weather endpoint works
- [ ] Create solution file: `dotnet new sln -n HRAgent`
- [ ] Add project to solution: `dotnet sln add HRAgent.Api/HRAgent.Api.csproj`

### Task 2: Initialize Frontend Project (AC: 2, 5, 7)
- [ ] Navigate to project root
- [ ] Run `npm create vite@latest hragent-ui -- --template react-ts`
- [ ] Navigate to hragent-ui directory
- [ ] Run `npm install` to install dependencies
- [ ] Verify node_modules populated correctly
- [ ] Add .gitignore for Node.js projects (node_modules, dist, .env)
- [ ] Test build: `npm run build` succeeds
- [ ] Test dev server: `npm run dev` starts on port 5173
- [ ] Verify React app loads in browser with Vite logo

### Task 3: Create Project Documentation (AC: 3, 4)
- [ ] Create root README.md with project overview
- [ ] Document backend setup instructions (commands, prerequisites)
- [ ] Document frontend setup instructions (Node.js version, npm commands)
- [ ] Add prerequisites section (.NET 10 SDK, Node.js 20+)
- [ ] Add "Quick Start" section with commands to run both projects
- [ ] Document port configuration (backend: 5000, frontend: 5173)
- [ ] Add troubleshooting section for common setup issues

### Task 4: Verify Project Structure (AC: All)
- [ ] Verify backend structure matches Minimal APIs pattern
- [ ] Verify frontend structure matches Vite React TypeScript template
- [ ] Test complete workflow: clone → install → build → run
- [ ] Verify .gitignore files prevent unwanted files from being tracked
- [ ] Commit initial project setup to git

---

## Dev Notes

### Critical Architecture Patterns

**Backend - Minimal APIs (NOT MVC Controllers):**
- The backend MUST use ASP.NET Core **Minimal APIs** architecture
- Program.cs should contain endpoint definitions, NOT Controllers folder
- Endpoint pattern: `app.MapGet("/endpoint", handler)` 
- This is critical for AG-UI protocol integration later (Story 2.2)
- ❌ DO NOT create Controllers/ folder or [ApiController] attributes
- ✅ DO use endpoint-based routing in Program.cs

**Frontend - Vite React TypeScript:**
- Use Vite (NOT Create React App - CRA is deprecated)
- TypeScript strict mode will be enabled in later story
- Component naming: PascalCase files (e.g., App.tsx, ChatInterface.tsx)
- This foundation enables CopilotKit integration (Story 2.4)

### Technology Stack Versions (from Architecture)

**Backend:**
- ✅ .NET 10 (NOT .NET 8 or 9) - C# 13 features required
- ✅ ASP.NET Core Minimal APIs (NO Controllers)
- Template: `dotnet new webapi` with `-f net10.0` flag

**Frontend:**
- ✅ React 18+ (comes with Vite template)
- ✅ TypeScript 5+ (comes with react-ts template)
- ✅ Vite 6+ (modern bundler - fast HMR)
- Template: `npm create vite@latest ... -- --template react-ts`

### Project Structure Requirements

**Backend Structure (from Architecture):**
```
HRAgent.Api/
├── Program.cs              # ✅ All endpoints defined here (Minimal APIs)
├── appsettings.json
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json
├── HRAgent.Api.csproj
└── .gitignore
```

**Frontend Structure (from Vite Template):**
```
hragent-ui/
├── src/
│   ├── main.tsx           # Entry point
│   ├── App.tsx            # Root component
│   ├── App.css
│   ├── index.css
│   └── vite-env.d.ts
├── public/
├── index.html
├── package.json
├── tsconfig.json
├── tsconfig.node.json
├── vite.config.ts
└── .gitignore
```

### Port Configuration (from Architecture)

**Default Ports:**
- Backend API: `http://localhost:5000` (configured in launchSettings.json)
- Frontend Dev Server: `http://localhost:5173` (Vite default)
- These ports will be used by .NET Aspire orchestration (Story 1.2)

### .gitignore Requirements

**Backend .gitignore must include:**
```gitignore
bin/
obj/
*.user
.vs/
.vscode/
appsettings.Development.json
```

**Frontend .gitignore must include:**
```gitignore
node_modules/
dist/
.env
.env.local
```

### Prerequisites

**Required Software:**
- .NET 10 SDK (verify: `dotnet --version` shows 10.x.x)
- Node.js 20+ (verify: `node --version` shows v20.x.x)
- npm 10+ (comes with Node.js 20)
- Git (for version control)

### Minimal APIs Example (from Architecture)

Program.cs should look like this after initialization:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Minimal APIs endpoint pattern
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            null
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
```

**Key Points:**
- ✅ NO Controllers folder
- ✅ Endpoints defined with `app.MapGet()`, `app.MapPost()`, etc.
- ✅ Top-level statements (no Program class)
- ✅ Record types for DTOs

### Testing Verification Steps

**Backend Verification:**
1. Run `dotnet build` - should complete successfully
2. Run `dotnet run` - should start Kestrel on port 5000
3. Visit http://localhost:5000/swagger - Swagger UI should load
4. Test `/weatherforecast` endpoint - should return JSON array
5. Press Ctrl+C to stop server

**Frontend Verification:**
1. Run `npm install` - should complete without errors
2. Run `npm run build` - should create dist/ folder
3. Run `npm run dev` - should start Vite dev server on port 5173
4. Visit http://localhost:5173 - should show Vite + React page
5. Hot Module Replacement (HMR) should work when editing App.tsx

### Common Gotchas & Anti-Patterns

**❌ DO NOT:**
- Use `dotnet new mvc` (wrong template - MVC Controllers)
- Create Controllers/ folder in backend
- Use Create React App (`npx create-react-app`) - it's deprecated
- Commit node_modules/ or bin/obj/ directories
- Hardcode connection strings in appsettings.json yet

**✅ DO:**
- Use `dotnet new webapi` with Minimal APIs
- Use `npm create vite@latest` with react-ts template
- Verify .gitignore files before first commit
- Document setup instructions in README.md
- Test complete workflow before marking story done

### References

**Architecture Document:**
- [Starter Template Evaluation](../architecture.md#starter-template-evaluation)
- [Minimal APIs Pattern](../architecture.md#minimal-apis-endpoint-organization)
- [Project Structure](../architecture.md#complete-project-directory-structure)

**Epic Context:**
- [Epic 1: Project Foundation](../project-planning-artifacts/epics.md#epic-1-project-foundation--development-environment)
- Foundation enables all subsequent user-facing features

**Project Context:**
- [Technology Stack & Versions](../project-context.md#technology-stack--versions)
- Critical version constraints: .NET 10, Minimal APIs, Vite 6+

---

## Dev Agent Record

### Completion Notes
Story file created with comprehensive context for flawless implementation. Developer has everything needed:
- ✅ Exact initialization commands with correct flags
- ✅ Architecture patterns (Minimal APIs, NOT Controllers)
- ✅ Project structure requirements
- ✅ Testing verification steps
- ✅ Common anti-patterns to avoid
- ✅ References to source documents

### Key Success Factors
1. **Minimal APIs Architecture** - Backend MUST use endpoint-based routing (no Controllers)
2. **Correct Templates** - `dotnet new webapi` + Vite react-ts (not CRA)
3. **Version Compliance** - .NET 10, Node.js 20+, Vite 6+
4. **Documentation** - README.md with complete setup instructions
5. **Verification** - Both projects build and run successfully

### File List
Files to be created by this story:
- `/HRAgent.Api/Program.cs`
- `/HRAgent.Api/HRAgent.Api.csproj`
- `/HRAgent.Api/appsettings.json`
- `/HRAgent.Api/.gitignore`
- `/hragent-ui/src/main.tsx`
- `/hragent-ui/src/App.tsx`
- `/hragent-ui/package.json`
- `/hragent-ui/vite.config.ts`
- `/hragent-ui/.gitignore`
- `/README.md`
- `/HRAgent.sln`

### Next Story
After completion, Story 1.2 will configure .NET Aspire orchestration to run both projects with a single command.
