---
project_name: 'bmad'
user_name: 'Alberto'
date: '2025-12-25'
sections_completed: ['technology_stack', 'language_framework', 'testing', 'code_quality', 'critical_rules']
status: 'complete'
rule_count: 75
optimized_for_llm: true
existing_patterns_found: 38
workflowType: 'project-context'
lastStep: 3
---

# Project Context for AI Agents

_This file contains critical rules and patterns that AI agents must follow when implementing code in this project. Focus on unobvious details that agents might otherwise miss._

---

## Technology Stack & Versions

**Backend - .NET 10:**
- C# 13 with ASP.NET Core Minimal APIs (endpoint-based routing, NO Controllers)
- Microsoft.Agents.AI (prerelease) - Microsoft Agent Framework for conversational AI
- Microsoft.Agents.AI.Hosting.AGUI.AspNetCore (prerelease) - AG-UI protocol with SSE streaming
- Microsoft.EntityFrameworkCore.Cosmos 10.0.0 - NoSQL with partition key strategies
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.0
- Microsoft.Identity.Web 3.3.1+ - Azure AD JWT token validation
- Azure.Storage.Blobs 12.25.0+ - Append blobs for immutable audit logging
- Polly 8.5.0+ - Resilience policies (retry, circuit breaker, timeout)
- Azure.AI.OpenAI 2.1.0+ - GPT-4o integration via Azure AI Foundry
- Microsoft.ApplicationInsights.AspNetCore 2.22.0+ - Telemetry and observability

**Frontend - React 18+:**
- TypeScript 5+ with strict mode enabled (tsconfig.json strictNullChecks, strictFunctionTypes)
- Vite 6+ - Build tool and dev server (NOT Create React App, NOT Webpack)
- CopilotKit 1.5.0+ - AG-UI protocol client with SSE EventSource
- Zustand 5.0.2+ - Domain-specific state stores (NOT Redux, NOT Context API for global state)
- Tailwind CSS 4+ with shadcn/ui component library
- @azure/msal-browser + @azure/msal-react - Azure AD authentication and token management

**Development & Infrastructure:**
- .NET Aspire 10.0+ - Local orchestration (single `dotnet run` starts backend + frontend + Cosmos emulator + Azurite)
- Azure Container Apps - Deployment target (separate containers: API 1-10 replicas, UI 1-3 replicas)
- Azure Cosmos DB Serverless - NoSQL database (conversations container with threadId partition, user-patterns with userId partition)
- Azure Blob Storage - Append blobs with 7-year immutability policy for audit logs

**CRITICAL Version Constraints:**
- ⚠️ MUST use .NET 10 (not .NET 8 or 9) - C# 13 features and Minimal APIs enhancements required
- ⚠️ MUST use Minimal APIs architecture - NO Controller-based routing, NO [ApiController] attributes
- ⚠️ MUST use Polly 8.5.0+ - v8 has breaking API changes from v7 (ResiliencePipeline vs Policy)
- ⚠️ MUST use CopilotKit 1.5.0+ - AG-UI protocol compatibility required for SSE streaming
- ⚠️ MUST use Zustand 5.0.2+ - v5 has breaking changes from v4 (no more `create()` wrapper)
- ⚠️ MUST use Vite 6+ - NOT Create React App (CRA is deprecated)

## Critical Implementation Rules

### Language & Framework-Specific Rules

**C# / .NET Minimal APIs Patterns:**

- ✅ MUST use Minimal APIs with endpoint-based routing in Program.cs - NO Controllers folder, NO [ApiController] attributes
- ✅ MUST use MapGroup() to organize endpoints by domain (e.g., `/conversations`, `/admin`, `/integrations`)
- ✅ ALWAYS use async/await for I/O operations (database, HTTP calls, file operations)
- ✅ MUST use Results.Created(), Results.Ok(), Results.NotFound() for endpoint responses
- ✅ MUST use dependency injection via endpoint parameters, NOT constructor injection
- ❌ FORBIDDEN: Creating Controller classes with [ApiController] or [Route] attributes
- ❌ FORBIDDEN: Using synchronous I/O methods (SaveChanges vs SaveChangesAsync)

**TypeScript / React Patterns:**

- ✅ MUST enable TypeScript strict mode (strictNullChecks, noImplicitAny, strictFunctionTypes)
- ✅ ALWAYS explicitly type React hooks: `useState<Type>()`, `useEffect()` with typed dependencies
- ✅ MUST use React.FC<PropsType> or explicit return types for function components
- ✅ MUST define interfaces for all props, state, and API responses
- ❌ FORBIDDEN: Using 'any' type anywhere in the codebase - always define proper types
- ❌ FORBIDDEN: Implicit typing that defeats TypeScript's safety guarantees

**CopilotKit AG-UI Protocol Integration:**

- ✅ MUST use CopilotKit with runtimeUrl pointing to SSE endpoint (e.g., `/api/copilotkit`)
- ✅ MUST implement EventSource for server-sent events (SSE) streaming
- ✅ MUST handle AG-UI protocol messages following Microsoft Agent Framework patterns
- ✅ MUST configure CopilotChat with appropriate labels and placeholders
- ❌ FORBIDDEN: Using polling or WebSockets instead of SSE for AG-UI protocol

**Zustand State Management (v5):**

- ✅ MUST create domain-specific stores (conversationStore, authStore, NOT a single global store)
- ✅ MUST use Zustand v5 syntax: `create<StoreType>((set) => ({ ... }))` - NO create()() wrapper
- ✅ MUST define TypeScript interfaces for all store shapes
- ✅ SHOULD use shallow comparison for performance: `useStore(state => state.field, shallow)`
- ❌ FORBIDDEN: Using Zustand v4 syntax with double create()() wrapper
- ❌ FORBIDDEN: Using Redux or Context API for global state (Zustand is the chosen solution)

**Error Handling Patterns:**

- ✅ MUST use global error middleware in Program.cs for unhandled exceptions
- ✅ MUST return Problem Details format for API errors (RFC 7807)
- ✅ MUST use try-catch with specific error handling in React components
- ✅ MUST log all errors to Application Insights with proper context

### Testing Rules

**Test Project Organization:**

- ✅ MUST create separate test projects: `HRAgent.Api.Tests/` for backend, `HRAgent.UI.Tests/` for frontend
- ✅ MUST mirror source structure in test projects (Endpoints/, Services/, components/, stores/)
- ✅ MUST use xUnit for .NET tests, Vitest (NOT Jest) for React tests with Vite
- ✅ MUST separate unit tests from integration tests in different files

**Backend Testing Patterns (.NET):**

- ✅ MUST use test naming: `MethodName_StateUnderTest_ExpectedBehavior`
- ✅ MUST use WebApplicationFactory<Program> for integration testing API endpoints
- ✅ MUST use InMemory database provider for unit tests - NEVER real Cosmos DB in tests
- ✅ MUST mock external dependencies (Factorial API, Azure OpenAI, Calendar APIs) using Moq or NSubstitute
- ✅ MUST test both happy path AND error cases for all endpoints
- ❌ FORBIDDEN: Mixing unit and integration tests in the same test class

**Frontend Testing Patterns (React):**

- ✅ MUST use Vitest + React Testing Library (NOT Jest with Vite projects)
- ✅ MUST use descriptive test names: `'displays error message when API call fails'`
- ✅ MUST test user-facing behavior, NOT internal implementation details (state, methods)
- ✅ MUST mock Zustand stores and API calls using vi.fn() or msw (Mock Service Worker)
- ✅ MUST use `screen.getByRole()` queries for accessibility compliance
- ❌ FORBIDDEN: Testing component internal state or private methods
- ❌ FORBIDDEN: Using enzyme or shallow rendering - use full React Testing Library render

**Test Coverage Requirements:**

- ✅ MUST achieve minimum 70% code coverage for backend services and endpoints
- ✅ MUST test all critical user flows with integration tests (auth, conversation creation, HR queries)
- ✅ SHOULD achieve 60%+ coverage for frontend components with business logic
- ✅ MUST test error boundaries and fallback UI components

### Code Quality & File Organization

**File & Folder Naming Conventions:**

**Backend (.NET):**
- ✅ MUST use PascalCase for all C# files (ConversationEndpoints.cs, AgentService.cs)
- ✅ MUST organize endpoints in separate files under Endpoints/ folder
- ✅ MUST keep one service per file in Services/ folder
- ✅ MUST use namespace matching folder structure: `namespace HRAgent.Api.Services;`
- ❌ FORBIDDEN: Putting all endpoints in a single massive Program.cs (use MapGroup extension methods)

**Frontend (React):**
- ✅ MUST use PascalCase for React component files (ChatInterface.tsx, MessageBubble.tsx)
- ✅ MUST use camelCase for non-component files (conversationStore.ts, formatters.ts)
- ✅ MUST prefix custom hooks with 'use' (useConversation.ts, useAuth.ts)
- ❌ FORBIDDEN: Using kebab-case for React component files (chat-interface.tsx is WRONG)

**API Endpoint Naming:**

- ✅ MUST use root-level endpoints WITHOUT /api prefix (e.g., `/conversations`, `/admin`)
- ✅ MUST use MapGroup() to organize related endpoints
- ❌ FORBIDDEN: Adding /api prefix to routes (e.g., `/api/conversations` is WRONG)

**Cosmos DB JSON Naming:**

- ✅ MUST use camelCase for JSON property names in Cosmos DB
- ✅ MUST use [JsonPropertyName("camelCase")] attributes on C# properties
- ✅ Example: C# property `ThreadId` maps to JSON `"threadId"` in database
- ❌ FORBIDDEN: Using PascalCase in JSON stored in Cosmos DB

**Import/Export Conventions:**

**TypeScript:**
- ✅ MUST use named exports for utilities, stores, hooks: `export const formatDate = ...`
- ✅ MUST use default export for React components: `export default function Component() { }`
- ❌ FORBIDDEN: Default exports for utility functions or stores

**C#:**
- ✅ MUST declare namespace matching folder structure at top of every file
- ✅ MUST use public/private/internal access modifiers explicitly
- ❌ FORBIDDEN: File-scoped types without namespace declaration

**Documentation Requirements:**

- ✅ MUST add XML comments (`///`) to all public C# APIs, services, and methods
- ✅ MUST add JSDoc comments to exported TypeScript functions with complex logic
- ✅ MUST document all environment variables and configuration in README.md
- ✅ SHOULD add inline comments for complex business logic and algorithms

### Critical Anti-Patterns & Gotchas

**Architecture Anti-Patterns:**

- ❌ FORBIDDEN: Creating Controllers/ folder or using [ApiController] attributes (this is Minimal APIs, not MVC)
- ❌ FORBIDDEN: Using Polly v7 Policy syntax - MUST use v8 ResiliencePipeline with AddRetry/AddCircuitBreaker
- ❌ FORBIDDEN: Using Zustand v4 double create()() wrapper - MUST use v5 single create<Type>() syntax
- ❌ FORBIDDEN: Using Create React App or Webpack - MUST use Vite 6+ as specified

**Database & Cosmos DB Gotchas:**

- ❌ FORBIDDEN: Cross-partition queries without explicit partition key (expensive and slow)
- ❌ FORBIDDEN: Using PascalCase in JSON stored in Cosmos DB - MUST use camelCase with [JsonPropertyName]
- ✅ MUST always include partition key (threadId or userId) in Cosmos queries for performance
- ✅ MUST use InMemory database provider for unit tests, NOT real Cosmos DB

**Security Critical Rules:**

- ❌ FORBIDDEN: Hardcoding secrets or API keys in code or appsettings files
- ❌ FORBIDDEN: Exposing internal error details (stack traces, exception messages) to clients
- ❌ FORBIDDEN: Using CORS with AllowAnyOrigin() in production environments
- ❌ FORBIDDEN: Committing appsettings.Development.json with real secrets to git
- ✅ MUST validate JWT tokens on all protected endpoints using Microsoft.Identity.Web
- ✅ MUST load all secrets from Azure Key Vault using builder.Configuration
- ✅ MUST return generic error messages to clients while logging detailed errors internally
- ✅ MUST use [Authorize] attribute on all endpoints except health checks and public endpoints

**Performance Anti-Patterns:**

- ❌ FORBIDDEN: N+1 query patterns - use .Include() or batch queries instead
- ❌ FORBIDDEN: Synchronous I/O blocking with .Result or .Wait() - use async/await everywhere
- ❌ FORBIDDEN: Polling for updates instead of using SSE (Server-Sent Events) for AG-UI protocol
- ✅ MUST use async/await for all I/O operations (database, HTTP calls, file operations)
- ✅ MUST implement Polly retry policies with exponential backoff for external API calls
- ✅ MUST use connection pooling and proper HttpClient lifecycle management

**AG-UI Protocol Critical Rules:**

- ❌ FORBIDDEN: Using polling or WebSockets instead of SSE (Server-Sent Events) for streaming
- ❌ FORBIDDEN: Not implementing error handling and reconnection logic for SSE connections
- ✅ MUST use EventSource for SSE streaming from CopilotKit runtime endpoint
- ✅ MUST handle EventSource onerror events with proper reconnection logic
- ✅ MUST implement timeout handling for long-running agent operations

**Deployment & Configuration Gotchas:**

- ❌ FORBIDDEN: Deploying without health check endpoints (/health and /ready)
- ❌ FORBIDDEN: Missing Application Insights configuration for production monitoring
- ❌ FORBIDDEN: Not implementing correlation IDs for distributed tracing
- ✅ MUST configure health checks for Cosmos DB, Blob Storage, and external dependencies
- ✅ MUST implement structured logging with correlation IDs for request tracing
- ✅ MUST use separate Azure Container Apps for backend API and frontend UI
- ✅ MUST configure .NET Aspire for local development orchestration

**Testing Gotchas:**

- ❌ FORBIDDEN: Testing React component internal state or implementation details
- ❌ FORBIDDEN: Mixing unit tests and integration tests in the same test class
- ❌ FORBIDDEN: Using Jest with Vite projects (use Vitest instead)
- ✅ MUST test user-facing behavior and accessibility with screen.getByRole() queries
- ✅ MUST mock all external dependencies in unit tests (Factorial API, Azure OpenAI, Calendar APIs)

---

## Usage Guidelines

**For AI Agents:**

- Read this file before implementing any code in this project
- Follow ALL rules exactly as documented - no exceptions
- When in doubt, prefer the more restrictive option (e.g., use async, add types, follow naming conventions)
- Refer to the architecture document (_bmad-output/architecture.md) for complete architectural context
- Update this file if new patterns or anti-patterns emerge during implementation

**For Humans:**

- Keep this file lean and focused on unobvious rules that AI agents need
- Update when technology stack changes or new patterns are established
- Review quarterly to remove rules that have become obvious or outdated
- Remove redundant information to maintain LLM context efficiency

**Last Updated:** 2025-12-25
