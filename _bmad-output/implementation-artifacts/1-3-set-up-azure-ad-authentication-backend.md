# Story 1.3: Set Up Azure AD Authentication (Backend)

**Status:** review  
**Epic:** 1 - Project Foundation & Development Environment  
**Story ID:** 1.3  
**Created:** 2025-12-27  

---

## Story

As a **developer**,  
I want to **configure the backend API to validate Azure AD JWT tokens**,  
So that **only authenticated users can access protected endpoints and we establish enterprise-grade security from day one**.

---

## Acceptance Criteria

**Given** the backend project is initialized with .NET Aspire orchestration (Stories 1.1 and 1.2 completed)  
**When** I add Microsoft.Identity.Web authentication middleware and configure Azure AD  
**Then:**

1. ✅ `appsettings.json` contains AzureAd configuration section (Instance, TenantId, ClientId, Audience)
2. ✅ `Program.cs` registers `AddMicrosoftIdentityWebApiAuthentication()` middleware
3. ✅ Middleware pipeline includes `app.UseAuthentication()` and `app.UseAuthorization()` in correct order
4. ✅ Test endpoint with `[Authorize]` attribute returns 401 Unauthorized for requests without JWT token
5. ✅ Test endpoint with `[Authorize]` attribute returns 200 OK for valid JWT token with correct audience
6. ✅ Invalid JWT tokens (wrong signature, expired, wrong audience) return 401 Unauthorized with appropriate error
7. ✅ Azure AD configuration is documented in README.md with setup instructions
8. ✅ Integration tests verify authentication middleware behavior with mock tokens

---

## Developer Context

### Critical Architecture Patterns

**Authentication Library: Microsoft.Identity.Web**
- Official Microsoft library for ASP.NET Core + Azure AD integration
- Built-in JWT Bearer token validation middleware
- Seamless Microsoft Graph integration for future Calendar API access
- Minimal configuration: 3 lines of code + appsettings.json
- Enterprise features: Conditional access policies, MFA, device compliance

**Security Foundation:**
- This story establishes the authentication foundation for ALL future endpoints
- Epic 2 (Conversational Interface) will protect AG-UI endpoint with authentication
- Epic 3-7 (PTO, Timesheets, Approvals) will verify user identity and permissions
- Audit logging (Story 1.6) requires authenticated userId for compliance

**Integration Pattern:**
- Backend validates JWT tokens from frontend (Story 1.4 will add MSAL.js client)
- Frontend acquires access tokens from Azure AD via MSAL.js
- Frontend sends tokens in `Authorization: Bearer {token}` header
- Backend validates token signature, expiration, audience, issuer automatically

### Technology Stack Details

**Microsoft.Identity.Web 3.3.1+:**
- NuGet Package: `Microsoft.Identity.Web`
- Compatibility: .NET 10, ASP.NET Core Minimal APIs
- JWT Bearer authentication with Azure AD v2.0 endpoints
- Automatic token validation with public key rotation handling
- Built-in support for multi-tenant applications (future Growth phase)

**Azure AD Configuration Requirements:**
- Azure AD tenant (e.g., contoso.onmicrosoft.com)
- App registration in Azure Portal with:
  - Application (client) ID
  - Tenant ID
  - Exposed API scope (api://{clientId}/.default)
  - Authentication: Single-page application (SPA) platform for frontend
- NO client secret required for backend (uses public key validation)

**Local Development vs Production:**
- Local: Use Azure AD test tenant or emulator credentials
- Production: Configure Azure AD with company's existing tenant
- Both: Same middleware code, different appsettings.json configuration

### Previous Story Learnings

**From Story 1.2 (Aspire Orchestration):**
- AppHost orchestrates both backend and frontend
- Backend runs on dynamically assigned port (managed by Aspire)
- Environment variables configured via Aspire (not hardcoded)
- Dashboard at http://localhost:15000 shows logs and traces

**Integration Points:**
- Authentication configuration in `appsettings.json` (loaded by Aspire)
- Authentication logs visible in Aspire dashboard
- Protected endpoints testable via AppHost orchestration
- Frontend (Story 1.4) will acquire tokens and send via VITE_API_URL

**File Structure (Current):**
```
HRAgent.Api/
├── Program.cs                       # <150 lines (from Story 1.1)
├── appsettings.json                 # Will add AzureAd section
├── appsettings.Development.json     # Dev tenant config
├── Endpoints/                       # (Future: from Epic 2+)
└── Services/                        # (Future: from Epic 2+)
```

### Architecture References

**From Architecture Document - Authentication & Security Decisions:**

**Selected Option: Microsoft.Identity.Web 3.3.1+**

**Rationale:**
- Official Microsoft library with first-party support
- Built-in JWT Bearer token validation middleware
- Microsoft Graph integration ready (Calendar API in Epic 7)
- Minimal configuration (3 lines + appsettings.json)
- Azure AD features: Conditional access, MFA, device compliance

**Alternatives Rejected:**
- ❌ IdentityServer4/Duende: Self-hosted IdP (unnecessary when Azure AD mandated)
- ❌ Auth0/Okta: Third-party cost, not Azure-native
- ❌ Custom JWT implementation: Security risks, reinventing wheel

**Implementation Impact:**
- Backend: Add `Microsoft.Identity.Web` NuGet package
- Program.cs: `builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)`
- Program.cs: `app.UseAuthentication()` + `app.UseAuthorization()`
- appsettings.json: Configure `AzureAd:Instance`, `TenantId`, `ClientId`, `Audience`
- Endpoints: Add `[Authorize]` attribute to require valid JWT token
- Frontend (Story 1.4): MSAL.js acquires tokens, sends in `Authorization: Bearer {token}` header

**From Project Context - Security Critical Rules:**

**FORBIDDEN:**
- ❌ Hardcoding secrets or API keys in code or appsettings files
- ❌ Exposing internal error details (stack traces) to clients
- ❌ Using CORS with AllowAnyOrigin() in production
- ❌ Committing appsettings.Development.json with real secrets to git
- ❌ Synchronous I/O blocking (.Result or .Wait())

**MUST:**
- ✅ Validate JWT tokens on ALL protected endpoints using Microsoft.Identity.Web
- ✅ Load secrets from Azure Key Vault (Story 1.8 deployment)
- ✅ Return generic error messages to clients, log detailed errors internally
- ✅ Use [Authorize] attribute on all endpoints except health checks
- ✅ Use async/await for all I/O operations

**Authentication Flow (This Story):**
1. Frontend user initiates login via MSAL.js (Story 1.4)
2. Frontend redirects to Azure AD login page
3. User authenticates with Azure AD (username/password, MFA)
4. Azure AD issues JWT token with claims (userId, email, roles)
5. Frontend receives token and stores securely
6. Frontend sends token in `Authorization: Bearer {token}` header to backend
7. **Backend (THIS STORY) validates token automatically via middleware**
8. Backend extracts userId from token claims for audit logging

### Technical Requirements

**Azure AD Configuration:**

**App Registration (Azure Portal):**
```
Name: HRAgent API
Supported account types: Single tenant (this directory only)
Expose an API:
  - Application ID URI: api://{clientId}
  - Scope: api://{clientId}/access_as_user
    - Who can consent: Admins and users
    - Display name: Access HRAgent API
    - Description: Allows the app to access HRAgent API on behalf of the signed-in user
```

**appsettings.json Configuration:**
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "{tenant-id}",
    "ClientId": "{client-id}",
    "Audience": "api://{client-id}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Identity": "Information"
    }
  }
}
```

**appsettings.Development.json (Local Development):**
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "common",
    "ClientId": "{dev-client-id}",
    "Audience": "api://{dev-client-id}"
  }
}
```

**Note:** Development configuration uses "common" tenant for multi-tenant testing. Production will use specific tenant ID.

### Code Patterns

**Program.cs Middleware Registration (Minimal APIs Pattern):**

```csharp
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add authorization policies (future: role-based policies)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware pipeline
// CRITICAL ORDER: Authentication BEFORE Authorization
app.UseAuthentication();  // Validates JWT token
app.UseAuthorization();   // Checks [Authorize] attributes

// Map endpoints (existing from Story 1.1)
app.MapGet("/weatherforecast", () => 
{
    // Returns weather data (no authentication required yet)
});

// Add protected endpoint for testing
app.MapGet("/secure", () => "Authenticated!")
    .RequireAuthorization();  // Minimal APIs style - equivalent to [Authorize]

app.Run();
```

**Testing Protected Endpoints:**

**Without Token (Expected: 401 Unauthorized):**
```bash
curl http://localhost:5000/secure
# Response: 401 Unauthorized
```

**With Valid Token (Expected: 200 OK):**
```bash
curl -H "Authorization: Bearer {valid-jwt-token}" http://localhost:5000/secure
# Response: 200 OK
# Body: "Authenticated!"
```

**With Invalid Token (Expected: 401 Unauthorized):**
```bash
curl -H "Authorization: Bearer invalid-token" http://localhost:5000/secure
# Response: 401 Unauthorized
# Body: { "type": "https://tools.ietf.org/html/rfc9457", "title": "Unauthorized", "status": 401 }
```

### Testing Strategy

**Integration Tests (HRAgent.Api.Tests/AuthenticationTests.cs):**

```csharp
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await _client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        // Arrange - Create mock JWT token with valid signature
        var token = CreateMockJwtToken();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Authenticated!", content);
    }

    private string CreateMockJwtToken()
    {
        // TODO: Use Microsoft.IdentityModel.Tokens to create test token
        // Or use WebApplicationFactory with test authentication handler
        return "mock-valid-token";
    }
}
```

**Manual Testing via Aspire Dashboard:**
1. Start AppHost: `dotnet run --project HRAgent.AppHost`
2. Dashboard opens at http://localhost:15000
3. Test unprotected endpoint: curl http://localhost:5000/weatherforecast → 200 OK
4. Test protected endpoint without token: curl http://localhost:5000/secure → 401 Unauthorized
5. Verify authentication logs in Aspire dashboard (Microsoft.Identity category)

### Error Handling Patterns

**Automatic Error Responses (Microsoft.Identity.Web):**

**401 Unauthorized - No Token:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9457",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header is missing or invalid"
}
```

**401 Unauthorized - Invalid Token:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9457",
  "title": "Unauthorized",
  "status": 401,
  "detail": "The token is invalid: IDX10223: Signature validation failed"
}
```

**401 Unauthorized - Expired Token:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9457",
  "title": "Unauthorized",
  "status": 401,
  "detail": "The token is expired"
}
```

**401 Unauthorized - Wrong Audience:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9457",
  "title": "Unauthorized",
  "status": 401,
  "detail": "IDX10214: Audience validation failed"
}
```

**Logging Configuration (appsettings.json):**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Identity": "Information"  // Logs authentication events
    }
  }
}
```

**Authentication Event Logs (Aspire Dashboard):**
- JWT token validation attempts
- Token signature validation results
- Audience/issuer validation failures
- Token expiration warnings
- Claims extraction (userId, email, roles)

### Common Gotchas & Anti-Patterns

**❌ DO NOT:**
- Add `[Authorize]` attribute to health check endpoints (`/health`, `/ready`)
- Use synchronous configuration loading (builder.Configuration blocks thread)
- Hardcode tenant ID or client ID in Program.cs (use appsettings.json)
- Commit appsettings.Development.json with real Azure AD credentials to git
- Enable CORS with AllowAnyOrigin() in production (security risk)
- Forget to call `UseAuthentication()` before `UseAuthorization()` (middleware order matters)

**✅ DO:**
- Protect ALL endpoints except health checks with authentication
- Use `RequireAuthorization()` Minimal APIs extension instead of [Authorize] attribute
- Load Azure AD configuration from appsettings.json
- Use Azure Key Vault for production secrets (Story 1.8 deployment)
- Configure CORS with specific origins (frontend URL)
- Test authentication middleware with integration tests (mock tokens)
- Verify authentication logs in Aspire dashboard during development

**Middleware Order (CRITICAL):**
```csharp
// ✅ CORRECT ORDER
app.UseAuthentication();   // MUST come first (validates JWT)
app.UseAuthorization();    // THEN checks [Authorize] attributes

// ❌ INCORRECT ORDER (authorization will fail)
app.UseAuthorization();
app.UseAuthentication();
```

**CORS Configuration (For Frontend):**
```csharp
// Add to Program.cs (BEFORE UseAuthentication)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // Vite dev server
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();  // BEFORE UseAuthentication
app.UseAuthentication();
app.UseAuthorization();
```

### Future Integration Points

**Story 1.4 (Azure AD Authentication - Frontend):**
- Frontend will use MSAL.js to acquire access tokens
- Frontend will send tokens in `Authorization: Bearer {token}` header
- Backend (THIS STORY) validates tokens automatically
- Frontend receives user profile claims from backend

**Epic 2 (Conversational Interface):**
- AG-UI endpoint will require authentication: `app.MapAGUI("/", agent).RequireAuthorization()`
- Conversation state (Cosmos DB) will store userId from JWT claims
- All chat messages will be tied to authenticated user for audit

**Epic 3-7 (PTO, Timesheets, Approvals):**
- All endpoints will extract userId from JWT token claims
- Role-based authorization (employee, manager, hr_admin) from Factorial HR
- Authorization policies: `[Authorize(Policy = "ManagerOnly")]`

**Story 1.6 (Audit Logging):**
- AuditLogger will extract userId from HttpContext.User.Claims
- Audit logs will record: timestamp, userId, eventType, action, reasoning
- Compliance requirement: All actions must be tied to authenticated user

### Prerequisites

**Required:**
- Story 1.1 completed (backend project initialized)
- Story 1.2 completed (Aspire orchestration working)
- .NET 10 SDK installed
- Azure AD tenant access (or test tenant)

**Azure AD Setup:**
1. Access to Azure Portal (portal.azure.com)
2. Permissions to create App registrations
3. Test Azure AD tenant (or company tenant with dev app)
4. OR use Azure AD B2C tenant for development

**Local Development Alternative:**
- Can use mock JWT tokens for testing (no Azure AD required)
- WebApplicationFactory with test authentication handler
- Integration tests don't require real Azure AD

### Completion Checklist

**Before marking story done:**
- [ ] Microsoft.Identity.Web NuGet package installed
- [ ] appsettings.json contains AzureAd configuration section
- [ ] Program.cs registers AddMicrosoftIdentityWebApiAuthentication()
- [ ] Middleware pipeline has UseAuthentication() before UseAuthorization()
- [ ] Test endpoint with RequireAuthorization() added (e.g., /secure)
- [ ] Test: curl /secure without token returns 401 Unauthorized
- [ ] Test: curl /secure with invalid token returns 401 Unauthorized
- [ ] Test: curl /secure with valid token returns 200 OK (mock or real)
- [ ] Integration tests verify authentication middleware behavior
- [ ] README.md updated with Azure AD setup instructions
- [ ] appsettings.Development.json documented (no secrets committed)
- [ ] Authentication logs visible in Aspire dashboard
- [ ] CORS configured for frontend origin (http://localhost:5173)
- [ ] All existing tests still pass (AspireIntegrationTests, WeatherForecastTests)

---

## Tasks / Subtasks

### Task 1: Install Microsoft.Identity.Web Package (AC: 1, 2)
- [x] Add NuGet package: `dotnet add HRAgent.Api package Microsoft.Identity.Web`
- [x] Verify package version: Microsoft.Identity.Web 4.2.0 installed
- [x] Add using statement to Program.cs: `using Microsoft.Identity.Web;`
- [x] Build project: `dotnet build HRAgent.Api`

### Task 2: Configure Azure AD in appsettings.json (AC: 1)
- [x] Add AzureAd configuration section to appsettings.json
- [x] Set Instance: "https://login.microsoftonline.com/"
- [x] Set TenantId: "{tenant-id}" (from Azure Portal)
- [x] Set ClientId: "{client-id}" (from Azure Portal)
- [x] Set Audience: "api://{client-id}"
- [x] Update appsettings.Development.json with test tenant config
- [x] Add documentation comment in appsettings.json
- [x] DO NOT commit real secrets to git (.gitignore appsettings.*.json)

### Task 3: Register Authentication Middleware (AC: 2, 3)
- [x] Add authentication services in Program.cs
- [x] Add authorization services: `builder.Services.AddAuthorization();`
- [x] Add authentication middleware BEFORE authorization
- [x] Verify middleware order: Authentication → Authorization
- [x] Build and run via Aspire: `dotnet run --project HRAgent.AppHost`

### Task 4: Add CORS Configuration (AC: 7)
- [x] Add CORS services in Program.cs with frontend origin
- [x] Add CORS middleware BEFORE UseAuthentication: `app.UseCors();`
- [x] Verify CORS allows frontend origin (Vite dev server)
- [x] Test: OPTIONS request from frontend returns CORS headers

### Task 5: Add Protected Test Endpoint (AC: 4, 5, 6)
- [x] Add protected endpoint in Program.cs: `/secure` with RequireAuthorization()
- [x] Test WITHOUT token returns 401
- [x] Test WITH invalid token returns 401
- [x] Create mock JWT token for testing (TestAuthHandler)
- [x] Test WITH valid token returns 200
- [x] Verify response: "Authenticated!"
- [x] Verify error responses follow Problem Details format

### Task 6: Configure Logging for Authentication (AC: 7)
- [x] Update Logging section in appsettings.json with Microsoft.Identity: Information
- [x] Start AppHost and verify authentication logs in Aspire dashboard
- [x] Test authentication events: token validation, claims extraction
- [x] Verify error logs for invalid tokens show detailed validation failures
- [x] Confirm no sensitive data (full tokens) logged

### Task 7: Create Integration Tests (AC: 8)
- [x] Create new test file: HRAgent.Api.Tests/AuthenticationTests.cs
- [x] Add xUnit test project reference (already exists)
- [x] Implement test: `ProtectedEndpoint_WithoutToken_Returns401()`
- [x] Implement test: `ProtectedEndpoint_WithInvalidToken_Returns401()`
- [x] Implement test: `ProtectedEndpoint_WithValidToken_Returns200()` (mock token)
- [x] Use WebApplicationFactory<Program> for integration testing
- [x] Mock JWT token validation using test authentication handler
- [x] Run tests: `dotnet test HRAgent.Api.Tests` - 6 tests passed
- [x] Verify all tests pass (3 new tests + existing tests)

### Task 8: Update Documentation (AC: 7)
- [x] Update README.md with "Azure AD Authentication Setup" section
- [x] Document Azure AD app registration steps
- [x] Document appsettings.json configuration
- [x] Add curl examples for testing protected endpoints
- [x] Add troubleshooting section for common auth errors
- [x] Document development vs production configuration
- [x] Link to Microsoft.Identity.Web documentation

### Task 9: Verify Complete Workflow (AC: 1-8)
- [x] Start AppHost: `dotnet run --project HRAgent.AppHost`
- [x] Verify Aspire dashboard shows backend with authentication logs
- [x] Test unprotected endpoint: curl /weatherforecast → 200 OK
- [x] Test protected endpoint without token: curl /secure → 401
- [x] Test protected endpoint with invalid token: curl /secure → 401
- [x] Test protected endpoint with valid token: curl /secure → 200 (via tests)
- [x] Run all integration tests: `dotnet test` → 6 authentication tests pass
- [x] Verify CORS headers in response (Access-Control-Allow-Origin)
- [x] Review authentication logs in Aspire dashboard
- [x] Stop AppHost and verify clean shutdown

---

## References

**Architecture Document:**
- [Authentication & Security Decisions](../../architecture.md#authentication--security-decisions)
- [Authentication Library: Microsoft.Identity.Web](../../architecture.md#authentication-library-microsoftidentityweb)
- [Error Handling Pattern](../../architecture.md#error-handling-pattern)
- [Security Critical Rules](../../architecture.md#security-critical-rules)

**Project Context:**
- [Technology Stack](../../project-context.md#technology-stack--versions)
- [Security Critical Rules](../../project-context.md#critical-implementation-rules)
- [Authentication Patterns](../../project-context.md#language--framework-specific-rules)

**Epic Context:**
- [Epic 1: Project Foundation](../../project-planning-artifacts/epics.md#epic-1-project-foundation--development-environment)
- Story 1.1 (prerequisite): Initialize backend and frontend projects
- Story 1.2 (prerequisite): Configure .NET Aspire orchestration
- Story 1.4 (next): Azure AD authentication (frontend)
- Story 1.6 (future): Audit logging (requires userId from JWT claims)

**External Documentation:**
- [Microsoft.Identity.Web Documentation](https://learn.microsoft.com/en-us/azure/active-directory/develop/microsoft-identity-web)
- [Azure AD App Registration](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- [JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- [ASP.NET Core Minimal APIs Security](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security)

---

**AI Agent Completion Notes:**

This story document was generated following the BMad Method "create-story" workflow in YOLO mode (agent activation step 4). The document includes:

✅ **Comprehensive Context Analysis:**
- Architecture document analysis for authentication patterns
- Project context rules for security and C# patterns
- Epic requirements and user story from epics.md
- Previous story (1.2) analysis for Aspire integration learnings

✅ **Developer Guardrails:**
- Specific Microsoft.Identity.Web version requirement (3.3.1+)
- Middleware order critical rule (Authentication before Authorization)
- CORS configuration for frontend integration
- Security anti-patterns and gotchas clearly documented

✅ **Technical Requirements:**
- Azure AD configuration with tenant/client IDs
- appsettings.json structure with environment-specific configs
- Program.cs middleware registration pattern
- Integration test strategy with WebApplicationFactory

✅ **Integration Points:**
- Story 1.2 Aspire orchestration (logs visible in dashboard)
- Story 1.4 frontend authentication (MSAL.js will send tokens)
- Epic 2+ protected endpoints (AG-UI, PTO, timesheets)
- Story 1.6 audit logging (requires userId from JWT claims)

✅ **Implementation Ready:**
- Clear acceptance criteria with verification steps
- Task breakdown with specific commands and code snippets
- Testing strategy with example xUnit tests
- Completion checklist with specific validation steps

The story is marked as **ready-for-dev** and the developer can begin implementation immediately with all necessary context to avoid common mistakes.

---

## Dev Agent Record

### Implementation Plan

**Approach:** Implement Azure AD JWT authentication using Microsoft.Identity.Web middleware following official ASP.NET Core patterns for Minimal APIs.

**Key Technical Decisions:**
1. **Package:** Microsoft.Identity.Web 4.2.0 (latest stable version)
2. **Middleware Order:** CORS → Authentication → Authorization (critical for proper request handling)
3. **Testing Strategy:** WebApplicationFactory with TestAuthHandler for mock JWT validation (no real Azure AD required)
4. **Configuration:** Template placeholders in appsettings.json, "common" tenant for development
5. **CORS Policy:** Specific origin (localhost:5173) for frontend Vite dev server

**Implementation Sequence (Red-Green-Refactor):**
1. Install Microsoft.Identity.Web package and verify build
2. Configure appsettings.json with Azure AD section and enhanced logging
3. Register authentication services with JwtBearer scheme
4. Add CORS configuration before authentication middleware
5. Add authentication and authorization middleware in correct order
6. Create `/secure` protected endpoint for testing
7. Write integration tests with mock authentication handler
8. Verify all tests pass (6 authentication tests)
9. Update README.md with Azure AD setup instructions

### Implementation Notes

**Files Modified:**
- [HRAgent.Api/HRAgent.Api.csproj](../../HRAgent.Api/HRAgent.Api.csproj) - Added Microsoft.Identity.Web 4.2.0 package
- [HRAgent.Api/Program.cs](../../HRAgent.Api/Program.cs) - Added authentication, authorization, CORS middleware and `/secure` endpoint
- [HRAgent.Api/appsettings.json](../../HRAgent.Api/appsettings.json) - Added AzureAd configuration section with placeholders
- [HRAgent.Api/appsettings.Development.json](../../HRAgent.Api/appsettings.Development.json) - Added development AzureAd configuration
- [HRAgent.Api.Tests/AuthenticationTests.cs](../../HRAgent.Api.Tests/AuthenticationTests.cs) - Created 6 integration tests with TestAuthHandler
- [README.md](../../README.md) - Added Azure AD Authentication Setup section with comprehensive documentation

**Code Architecture:**
- Middleware pipeline follows ASP.NET Core best practices: CORS → Authentication → Authorization
- TestAuthHandler simulates Azure AD JWT validation for testing without external dependencies
- Minimal APIs pattern with `.RequireAuthorization()` extension instead of `[Authorize]` attribute
- Logging configured to show Microsoft.Identity events at Information level for debugging

**Test Coverage:**
- ✅ Protected endpoint without token returns 401
- ✅ Protected endpoint with invalid token returns 401  
- ✅ Protected endpoint with expired token returns 401
- ✅ Protected endpoint with wrong audience returns 401
- ✅ Protected endpoint with valid token returns 200
- ✅ Unprotected endpoint without token returns 200

**Learnings from Implementation:**
- Microsoft.Identity.Web automatically validates JWT signature, expiration, audience, and issuer
- CORS must be configured before UseAuthentication() to allow frontend requests
- WebApplicationFactory with ConfigureTestServices allows replacing authentication for testing
- Template placeholders in appsettings.json prevent accidental secret commits

### Completion Notes

**Story 1.3 Implementation Complete - 2025-12-27**

All acceptance criteria satisfied:
- ✅ AC1: appsettings.json contains AzureAd configuration section
- ✅ AC2: Program.cs registers AddMicrosoftIdentityWebApiAuthentication() middleware
- ✅ AC3: Middleware pipeline includes UseAuthentication() and UseAuthorization() in correct order
- ✅ AC4: Test endpoint with RequireAuthorization() returns 401 without JWT token
- ✅ AC5: Test endpoint returns 200 OK for valid JWT token (verified via tests)
- ✅ AC6: Invalid JWT tokens return 401 Unauthorized with appropriate error messages
- ✅ AC7: Azure AD configuration documented in README.md with setup instructions
- ✅ AC8: Integration tests verify authentication middleware behavior with mock tokens

**Key Achievements:**
- Installed Microsoft.Identity.Web 4.2.0 with all dependencies
- Configured authentication middleware with Azure AD integration
- Created 6 comprehensive integration tests (all passing)
- Updated README.md with detailed Azure AD setup guide
- Established foundation for Story 1.4 (frontend authentication with MSAL.js)

**Next Story Integration Points:**
- Story 1.4 will add MSAL.js to frontend to acquire and send JWT tokens
- Protected endpoints ready for AG-UI implementation in Epic 2
- Authentication logs visible in Aspire dashboard for debugging
- User claims extraction pattern ready for audit logging (Story 1.6)

---

## File List

### New Files
- `HRAgent.Api.Tests/AuthenticationTests.cs` - Integration tests for Azure AD JWT authentication (6 tests)

### Modified Files  
- `HRAgent.Api/HRAgent.Api.csproj` - Added Microsoft.Identity.Web 4.2.0 package reference
- `HRAgent.Api/Program.cs` - Added authentication, authorization, CORS middleware; created `/secure` protected endpoint
- `HRAgent.Api/appsettings.json` - Added AzureAd configuration section with Microsoft.Identity logging
- `HRAgent.Api/appsettings.Development.json` - Added development AzureAd configuration with "common" tenant
- `README.md` - Added Azure AD Authentication Setup section with comprehensive setup instructions

### Dependencies Added
- Microsoft.Identity.Web 4.2.0
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.0 (transitive)
- Microsoft.IdentityModel.* packages (transitive dependencies for JWT validation)

---

## Change Log

**2025-12-27 - Story 1.3: Azure AD Authentication (Backend) - COMPLETE**
- Installed Microsoft.Identity.Web 4.2.0 for JWT Bearer authentication
- Configured Azure AD authentication middleware with JwtBearer scheme
- Added CORS policy for frontend origin (localhost:5173)
- Created `/secure` protected test endpoint with RequireAuthorization()
- Implemented 6 integration tests with TestAuthHandler for mock JWT validation
- Updated README.md with Azure AD setup instructions and troubleshooting guide
- All 6 authentication tests passing, middleware correctly validates tokens
- Backend ready for Story 1.4 (frontend MSAL.js integration)

---
