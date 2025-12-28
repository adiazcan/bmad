---
stepsCompleted: [1, 2, 3, 4]
inputDocuments:
  - /home/adiaz/github/bmad/_bmad-output/prd.md
  - /home/adiaz/github/bmad/_bmad-output/architecture.md
  - /home/adiaz/github/bmad/_bmad-output/project-planning-artifacts/ux-design-specification.md
workflowType: 'epics-and-stories'
project_name: 'bmad'
user_name: 'Alberto'
date: '2025-12-25'
completionDate: '2025-12-25'
completionStatus: 'Complete'
totalEpics: 7
totalStories: 73
totalFRsCovered: 58
totalNFRsCovered: 40
---

# bmad - Epic Breakdown

## Overview

This document provides the complete epic and story breakdown for bmad (HRAgent), decomposing the requirements from the PRD, UX Design, and Architecture requirements into implementable stories.

## Requirements Inventory

### Functional Requirements

FR1: Employees can authenticate via Azure AD/SSO to access HRAgent
FR2: The system can read and respect role-based permissions from Factorial HR (employee, manager, HR admin)
FR3: The system can maintain secure session management across devices
FR4: The system can log all authentication events for audit purposes
FR5: Users can interact with HRAgent via web-based chat interface
FR6: Users can access chat interface from desktop and mobile browsers
FR7: The system can parse natural language date expressions ("next Friday", "March 10-12")
FR8: The system can provide real-time streaming responses via SSE
FR9: The system can maintain conversation context across multiple messages
FR10: The system can display rich formatted content (cards, buttons, lists)
FR11: Users can receive browser notifications for time-sensitive updates
FR12: Employees can submit PTO requests via conversational interface
FR13: Employees can check their current PTO balance
FR14: Employees can view status of pending PTO requests
FR15: Employees can cancel pending PTO requests
FR16: The system can validate PTO requests against company policies
FR17: The system can validate PTO requests against employee balances
FR18: Managers can receive notifications for team member PTO requests
FR19: Managers can view context-rich approval cards with employee details, dates, balance, and policy compliance
FR20: Managers can approve or deny PTO requests with optional comments
FR21: Employees can receive notifications when PTO requests are approved or denied
FR22: The system can synchronize PTO data with Factorial HR in real-time
FR23: Employees can log work hours via conversational interface
FR24: Employees can specify project/task allocation for logged hours
FR25: Employees can submit completed weekly timesheets
FR26: Employees can check timesheet submission status
FR27: Employees can view missing or incomplete timesheet entries
FR28: The system can send reminder notifications before timesheet deadlines
FR29: The system can synchronize timesheet data with Factorial HR
FR30: Employees can ask HR policy questions via conversational interface
FR31: The system can provide answers to policy questions using RAG-based knowledge retrieval
FR32: The system can cite source documents for policy answers
FR33: The system can escalate complex or ambiguous policy questions to HR administrators
FR34: HR administrators can view and respond to escalated policy questions
FR35: The system can track frequently asked questions for knowledge base improvement
FR36: The system can route approval requests to appropriate managers
FR37: The system can provide complete approval context (requestor, dates, balances, policies, reasons)
FR38: Managers can perform one-click approvals or denials
FR39: Managers can add comments to approval decisions
FR40: The system can notify requestors of approval decisions
FR41: The system can generate immutable audit logs for all transactions
FR42: The system can capture complete decision context (who, what, when, why, reasoning)
FR43: HR administrators can access complete audit trails for compliance reporting
FR44: The system can validate all transactions against configured policies
FR45: The system can flag policy violations for HR review
FR46: The system can ensure GDPR compliance for all data handling
FR47: The system can authenticate with Factorial HR API
FR48: The system can read employee data from Factorial HR (including roles and permissions)
FR49: The system can write PTO requests to Factorial HR
FR50: The system can write timesheet entries to Factorial HR
FR51: The system can sync data changes in near-real-time
FR52: The system can handle Factorial API errors gracefully with user feedback
FR53: HR administrators can access usage analytics dashboard
FR54: The system can track adoption metrics (users, requests, completions)
FR55: The system can track performance metrics (response times, completion rates, errors)
FR56: The system can track satisfaction metrics via pulse survey integration
FR57: HR administrators can view exception queues and handle edge cases
FR58: HR administrators can configure system policies and thresholds

### Non-Functional Requirements

**Performance:**
NFR-P1: User-initiated chat messages must receive first response within 3 seconds (p95)
NFR-P2: SSE streaming must begin time-to-first-token within 1 second of request
NFR-P3: Page load time must be under 2 seconds on 4G connection
NFR-P4: Time to Interactive (TTI) must be under 3 seconds
NFR-P5: Chat message rendering must complete within 100ms per message
NFR-P6: The system must support 200 concurrent users without performance degradation
NFR-P7: API latency p95 must remain under 3 seconds under normal load

**Security:**
NFR-S1: All data must be encrypted in transit using TLS 1.2 or higher
NFR-S2: All sensitive data must be encrypted at rest using Azure-managed encryption
NFR-S3: All API requests must be authenticated via Azure AD/SSO
NFR-S4: Secrets and credentials must be stored in Azure Key Vault (not in code or config files)
NFR-S5: The system must enforce role-based access control based on Factorial HR permissions
NFR-S6: All audit logs must be immutable and tamper-proof
NFR-S7: The system must comply with GDPR requirements for data handling, retention, and user rights
NFR-S8: Session tokens must expire after 8 hours of inactivity
NFR-S9: Failed authentication attempts must be logged and monitored

**Scalability:**
NFR-SC1: The system must support linear scaling from 200 to 500 users with <10% performance degradation
NFR-SC2: Azure Container Apps must auto-scale based on CPU and request volume metrics
NFR-SC3: The system must handle 10x daily usage spikes (e.g., Friday timesheet deadline) without service degradation
NFR-SC4: Database connections must be pooled and reused efficiently to support concurrent operations
NFR-SC5: LLM token consumption must be monitored and optimized to stay within budget projections

**Reliability & Availability:**
NFR-R1: The system must maintain >99.9% uptime during business hours (8am-6pm local time)
NFR-R2: Planned maintenance must be scheduled outside business hours with 48-hour advance notice
NFR-R3: SSE connections must automatically reconnect on network disruption without user intervention
NFR-R4: Failed API requests must be retried with exponential backoff (maximum 3 attempts)
NFR-R5: The system must gracefully degrade when Factorial API is unavailable (queue operations, notify users)
NFR-R6: System errors must be logged with complete context for troubleshooting
NFR-R7: Critical errors must trigger automated alerts to the operations team

**Integration Quality:**
NFR-I1: Factorial HR integration must maintain <1% failure rate for read operations
NFR-I2: Factorial HR integration must maintain <2% failure rate for write operations (with retry)
NFR-I3: Data synchronization with Factorial HR must complete within 5 seconds for user-initiated actions
NFR-I4: The system must handle Factorial API rate limits gracefully (implement caching and request batching)
NFR-I5: The system must detect and alert on Factorial API schema changes that could break integration
NFR-I6: All integration errors must provide actionable feedback to users (not generic error messages)

**Usability & User Experience:**
NFR-U1: The chat interface must be responsive and function correctly on mobile devices (320px minimum width)
NFR-U2: The system must provide clear feedback for all user actions within 300ms (loading indicators, confirmations)
NFR-U3: Error messages must be user-friendly with clear next steps (not technical stack traces)
NFR-U4: The system must maintain conversation context for at least 10 message exchanges
NFR-U5: Browser notifications must be configurable by users (enable/disable, timing preferences)
NFR-U6: The interface must support keyboard navigation for power users

**Observability & Monitoring:**
NFR-O1: All user interactions must be logged with complete context for analytics
NFR-O2: System performance metrics must be collected and visualized in real-time dashboards
NFR-O3: The system must track and report on success criteria metrics (adoption, satisfaction, time savings, completion rates)
NFR-O4: Application Insights must provide comprehensive telemetry for troubleshooting
NFR-O5: Cost metrics (LLM tokens, Azure resources) must be tracked and reported daily
NFR-O6: The system must generate weekly reports on usage patterns, errors, and performance trends

**Compliance & Audit:**
NFR-C1: All transactions must generate immutable audit log entries with timestamp, user, action, and reasoning
NFR-C2: Audit logs must be retained for minimum 7 years for compliance purposes
NFR-C3: The system must support audit trail export in standard formats (CSV, JSON)
NFR-C4: All policy violations must be flagged and logged for HR review
NFR-C5: The system must maintain data lineage for all decisions (trace back from outcome to input data)

### Additional Requirements

**From Architecture Document:**

- **Starter Template Specified**: Architecture specifies two starters:
  - Backend: `dotnet new webapi` (.NET 10 Minimal APIs) with Microsoft.Agents.AI packages
  - Frontend: Vite + React + TypeScript with CopilotKit for AG-UI integration
- Infrastructure deployment via Azure Container Apps with .NET Aspire for local orchestration
- Cosmos DB Serverless with NoSQL API for conversation state storage (threadId partition key)
- Azure Blob Storage with append blobs for immutable audit trail persistence (7-year retention)
- Microsoft.Identity.Web 3.3.1+ for Azure AD/SSO authentication
- Polly 8.5.0+ for Factorial API resilience (retry, circuit breaker, timeout)
- Zustand 5.0.2+ for frontend state management (domain-specific stores)
- Application Insights for observability and telemetry
- Separate containers for backend and frontend with independent scaling
- Minimal APIs pattern with endpoint groups in `/Endpoints` folder
- camelCase for all JSON/Cosmos DB properties, PascalCase for React components
- AG-UI protocol over HTTP with Server-Sent Events (SSE) for real-time streaming
- No caching in MVP (explicit decision - add after performance profiling)
- Pattern recognition storage in separate Cosmos DB container (userId partition key)

**From UX Design Document:**

- Mobile-first responsive design with 320px minimum width support
- Thumb-optimized touch targets (≥44px) for mobile interactions
- Desktop-rich context with multi-column layouts and data visualizations
- Streaming SSE responses with time-to-first-token <1s for engagement
- Optimistic UI with instant feedback and background sync
- Context-rich approval cards with team coverage, policy compliance, risk scoring
- Green/Yellow/Red confidence indicators with transparent reasoning
- Progressive disclosure: simple interface that reveals complexity as needed
- Zero-configuration personalization through silent pattern learning
- Graceful error handling with collaborative problem-solving framing
- Pattern recognition accuracy target: ≥85% for "log like last week" feature
- Intent recognition accuracy target: ≥90% for conversational understanding

### FR Coverage Map

**Epic 1 - Project Foundation & Development Environment (FRs 1-4, 47-48, 52):**
- FR1: Azure AD/SSO authentication
- FR2: RBAC from Factorial HR
- FR3: Secure session management
- FR4: Authentication event logging
- FR47: Factorial HR API authentication
- FR48: Read employee data from Factorial
- FR52: Graceful Factorial API error handling

**Epic 2 - Conversational Interface Foundation (FRs 5-11):**
- FR5: Web-based chat interface
- FR6: Desktop and mobile browser access
- FR7: Natural language date parsing
- FR8: Real-time SSE streaming responses
- FR9: Conversation context maintenance
- FR10: Rich formatted content display
- FR11: Browser notifications

**Epic 3 - PTO Management (FRs 12-22, 49, 51):**
- FR12: Submit PTO requests conversationally
- FR13: Check PTO balance
- FR14: View PTO request status
- FR15: Cancel pending PTO requests
- FR16: Validate against company policies
- FR17: Validate against employee balances
- FR18: Manager PTO request notifications
- FR19: Context-rich approval cards
- FR20: One-click approve/deny
- FR21: Approval decision notifications
- FR22: Real-time Factorial HR sync
- FR49: Write PTO requests to Factorial
- FR51: Near-real-time data sync

**Epic 4 - Timesheet Management (FRs 23-29, 50, 51):**
- FR23: Log work hours conversationally
- FR24: Specify project/task allocation
- FR25: Submit weekly timesheets
- FR26: Check timesheet submission status
- FR27: View missing/incomplete entries
- FR28: Reminder notifications before deadlines
- FR29: Synchronize timesheet data with Factorial
- FR50: Write timesheet entries to Factorial
- FR51: Near-real-time data sync

**Epic 5 - Policy Knowledge & Support (FRs 30-35):**
- FR30: Ask HR policy questions conversationally
- FR31: RAG-based policy answers
- FR32: Source document citations
- FR33: Escalate complex questions to HR
- FR34: HR admin view/respond to escalations
- FR35: Track FAQ for knowledge base improvement

**Epic 6 - Audit, Compliance & Monitoring (FRs 41-46, 53-58):**
- FR41: Generate immutable audit logs
- FR42: Capture complete decision context
- FR43: Access audit trails for compliance
- FR44: Validate transactions against policies
- FR45: Flag policy violations for review
- FR46: GDPR compliance for data handling
- FR53: Usage analytics dashboard
- FR54: Track adoption metrics
- FR55: Track performance metrics
- FR56: Track satisfaction metrics
- FR57: Exception queue handling
- FR58: Configure system policies

**Epic 7 - Manager Approval Workflows (FRs 36-40):**
- FR36: Route approval requests to managers
- FR37: Provide complete approval context
- FR38: One-click approval/denial
- FR39: Add comments to decisions
- FR40: Notify requestors of decisions

## Epic List

### Epic 1: Project Foundation & Development Environment
Development team can start building HRAgent with proper infrastructure, authentication, and development tools in place. Developers can initialize projects using official starter templates, run complete local development environment with one command, authenticate and deploy to Azure infrastructure. Foundation enables all subsequent user-facing features.

**FRs covered:** FR1, FR2, FR3, FR4, FR47, FR48, FR52

**Implementation notes:** Uses `dotnet new webapi` (Minimal APIs) + Vite React TypeScript starters, .NET Aspire orchestration for local dev, Azure Container Apps deployment, Azure DocumentDB (production) + MongoDB (local) with unified MongoDB.Driver & Blob Storage setup, Microsoft.Identity.Web for Azure AD/SSO, Polly for Factorial API resilience.

---

### Epic 2: Conversational Interface Foundation
Employees can interact with HRAgent through natural conversation on any device, establishing the core chat experience. Users can access HRAgent via web chat from desktop or mobile browser, have natural conversations with context maintained across messages, see real-time streaming AI responses, and receive clear feedback for all actions.

**FRs covered:** FR5, FR6, FR7, FR8, FR9, FR10, FR11

**Implementation notes:** AG-UI protocol with SSE streaming (<1s time-to-first-token), mobile-first responsive (320px min, ≥44px touch targets), CopilotKit integration for React frontend, conversation state in MongoDB (threadId indexed), ≥90% intent recognition accuracy target, natural date parsing ("next Friday", "March 10-12").

---

### Epic 3: PTO Management
Employees can request PTO conversationally, managers can approve with complete context, all synced with Factorial HR. Users can submit PTO requests via natural language ("I need March 10-12 off"), check PTO balance and request status, cancel pending requests. Managers receive approval notifications with context-rich cards and can approve/deny with one-click. All PTO data synchronized with Factorial HR in real-time.

**FRs covered:** FR12, FR13, FR14, FR15, FR16, FR17, FR18, FR19, FR20, FR21, FR22, FR49, FR51

**Implementation notes:** FactorialClient with Polly resilience (3 retries, circuit breaker), natural date parsing, context-rich approval cards (balance, policy compliance, team coverage), Green/Yellow/Red confidence indicators, real-time Factorial API sync (<5s), desktop-rich approval card visualization.

---

### Epic 4: Timesheet Management
Employees can log work hours conversationally with project allocation, submit weekly timesheets, and receive helpful reminders. Users can log hours via conversation ("Log 8 hours today on Marketing Campaign"), specify project/task allocation, submit weekly timesheets with confirmation, check submission status and missing entries, receive reminder notifications before deadlines. All timesheet data synchronized with Factorial HR.

**FRs covered:** FR23, FR24, FR25, FR26, FR27, FR28, FR29, FR50, FR51

**Implementation notes:** Conversational hour logging with project inference, pattern recognition storage (MongoDB user-patterns collection), reminder notifications (learned optimal timing in Growth phase), real-time Factorial sync, Friday deadline spike handling (10x capacity), mobile-first quick interactions.

---

### Epic 5: Policy Knowledge & Support (RAG)
Employees get instant, accurate answers to HR policy questions with source citations, reducing HR interruptions. Users can ask HR policy questions via conversation, receive accurate answers with source document citations, get escalated to HR admin for complex questions. HR admins can view and respond to escalated questions. System learns from frequently asked questions.

**FRs covered:** FR30, FR31, FR32, FR33, FR34, FR35

**Implementation notes:** RAG-based knowledge retrieval (Azure AI Search or MongoDB vector search), text-embedding-ada-002 for embeddings, source citation transparency, escalation workflow to HR admins, FAQ tracking for knowledge base improvement, confidence thresholds trigger escalation.

---

### Epic 6: Audit, Compliance & Monitoring
Complete compliance infrastructure with immutable audit trails, real-time monitoring, and analytics dashboards for HR administrators. All transactions automatically generate immutable audit logs. Complete decision traceability months later (who, what, when, why, reasoning). HR admins access audit trails for compliance reporting, view usage analytics and performance dashboards. Policy violations automatically flagged. GDPR-compliant data handling with export capability.

**FRs covered:** FR41, FR42, FR43, FR44, FR45, FR46, FR53, FR54, FR55, FR56, FR57, FR58

**Implementation notes:** Azure Blob Storage append blobs with 7-year immutability policy, JSON lines format for audit logs, Application Insights for observability, admin dashboard for exceptions and analytics, complete data lineage for all decisions, real-time monitoring of adoption/satisfaction/time savings.

---

### Epic 7: Manager Approval Workflows
Managers efficiently process approval requests with complete context assembly and intelligent recommendations. Managers receive approval requests with all decision context assembled, see team coverage visualization, policy compliance, risk scoring. Can approve or deny with one-click and optional comments. Requestors get notified of approval decisions. All approval decisions captured in audit trail with reasoning.

**FRs covered:** FR36, FR37, FR38, FR39, FR40

**Implementation notes:** Context assembly from multiple systems (Factorial, Calendar, Projects), desktop-rich approval cards with data visualization, Green/Yellow/Red confidence signals with transparent reasoning, one-click approval with <2 min average time, complete audit trail for every decision, builds upon Epic 3 & 4 workflows.

---

## Epic 1: Project Foundation & Development Environment

Development team can start building HRAgent with proper infrastructure, authentication, and development tools in place. Developers can initialize projects using official starter templates, run complete local development environment with one command, authenticate and deploy to Azure infrastructure. Foundation enables all subsequent user-facing features.

### Story 1.1: Initialize Backend and Frontend Projects

As a developer,
I want to initialize the backend API and frontend SPA projects using official starter templates,
So that the team has a working foundation to build upon.

**Acceptance Criteria:**

**Given** the project repository is empty
**When** I run the initialization commands
**Then** backend project is created using `dotnet new webapi -n HRAgent.Api -f net10.0`
**And** frontend project is created using Vite React TypeScript template
**And** both projects have proper .gitignore files
**And** README.md includes setup instructions
**And** projects can be built successfully (`dotnet build` and `npm install` work)

### Story 1.2: Configure .NET Aspire Orchestration

As a developer,
I want to run both backend and frontend with a single command,
So that local development is streamlined and services are properly connected.

**Acceptance Criteria:**

**Given** backend and frontend projects exist
**When** I create the Aspire AppHost project and run `dotnet run`
**Then** both backend API and frontend SPA start automatically
**And** backend runs on http://localhost:5000
**And** frontend runs on http://localhost:5173
**And** frontend receives backend API URL via environment variable
**And** Aspire dashboard is accessible at http://localhost:15000 showing logs and traces

### Story 1.3: Set Up Azure AD Authentication (Backend)

As a developer,
I want the backend API to validate Azure AD JWT tokens,
So that only authenticated users can access protected endpoints.

**Acceptance Criteria:**

**Given** backend project is initialized
**When** I add Microsoft.Identity.Web package and configure Azure AD
**Then** `appsettings.json` contains AzureAd section (Instance, TenantId, ClientId, Audience)
**And** `Program.cs` registers `AddMicrosoftIdentityWebApiAuthentication()`
**And** middleware pipeline includes `app.UseAuthentication()` and `app.UseAuthorization()`
**And** test endpoint with `[Authorize]` attribute returns 401 for unauthenticated requests
**And** test endpoint returns 200 for valid JWT token with correct audience

### Story 1.4: Set Up Azure AD Authentication (Frontend)

As a developer,
I want the frontend to acquire Azure AD access tokens,
So that authenticated API requests can be made to the backend.

**Acceptance Criteria:**

**Given** frontend project is initialized
**When** I implement MSAL.js authentication
**Then** `lib/auth.ts` exports `msalInstance` configured with Azure AD client ID and tenant ID
**And** `authStore.ts` Zustand store manages user and accessToken state
**And** `authStore` persists to localStorage for cross-tab token sharing
**And** `lib/api-client.ts` fetch wrapper adds `Authorization: Bearer {token}` header to all requests
**And** login flow redirects to Azure AD and returns with valid token
**And** token refresh works automatically when token expires

### Story 1.5: Configure Azure DocumentDB and MongoDB Connection

As a developer,
I want the backend to connect to Azure DocumentDB (production) and MongoDB (local) for conversation state storage,
So that user conversations can be persisted and retrieved with unified driver and environment parity.

**Acceptance Criteria:**

**Given** Azure DocumentDB cluster exists for production and MongoDB container for local
**When** I configure MongoDB.Driver
**Then** `MongoDB.Driver` NuGet package is installed
**And** `Aspire.MongoDB.Driver` package is installed for .NET Aspire integration
**And** `MongoDbService.cs` is created with `IMongoClient` and `IMongoDatabase` configuration
**And** `appsettings.json` contains MongoDB section (ConnectionString, DatabaseName)
**And** connection string is referenced from Azure Key Vault in production config
**And** `Program.cs` registers `IMongoClient` as singleton and `IMongoDatabase` as scoped
**And** health check endpoint `/ready` verifies MongoDB connectivity
**And** .NET Aspire orchestrates MongoDB container for local development
**And** same MongoDB.Driver code works for both local MongoDB and Azure DocumentDB

### Story 1.6: Configure Blob Storage for Audit Logs

As a developer,
I want the backend to write audit logs to Azure Blob Storage append blobs,
So that immutable compliance records are captured from day one.

**Acceptance Criteria:**

**Given** Azure Blob Storage account exists
**When** I configure Blob Storage client
**Then** `Azure.Storage.Blobs` package is installed
**And** `AuditLogger.cs` service is created with `LogAsync(eventType, userId, data)` method
**And** `appsettings.json` contains BlobStorage section (ConnectionString, ContainerName)
**And** `AuditLogger` creates append blobs in format `audit/{year}/{month}/{day}/{threadId}.jsonl`
**And** immutability policy (7-year retention) is configured on `audit-logs` container
**And** audit log writes are thread-safe using SemaphoreSlim
**And** Azurite blob emulator works for local development

### Story 1.7: Configure Factorial HR API Client with Polly

As a developer,
I want a resilient Factorial HR API client with retry and circuit breaker policies,
So that transient Factorial API failures don't break the application.

**Acceptance Criteria:**

**Given** Factorial HR API key is available
**When** I create `FactorialClient` with Polly policies
**Then** `Microsoft.Extensions.Http.Polly` package is installed
**And** `FactorialClient.cs` is created with HttpClient dependency
**And** `appsettings.json` contains Factorial section (BaseUrl, ApiKey reference)
**And** Factorial API key is stored in Azure Key Vault
**And** `Program.cs` registers HttpClient with 3 Polly policies:
  - Retry policy: 3 attempts with exponential backoff
  - Circuit breaker: Opens after 5 consecutive failures
  - Timeout policy: 2 seconds per request
**And** `FactorialClient` has placeholder methods: `GetEmployeeAsync()`, `GetPTOBalanceAsync()`
**And** Polly policies log retry attempts and circuit breaker state changes to Application Insights

### Story 1.8: Deploy Infrastructure to Azure Container Apps

As a developer,
I want to deploy backend and frontend to Azure Container Apps,
So that the application runs in production Azure environment.

**Acceptance Criteria:**

**Given** Dockerfiles exist for backend and frontend
**When** I deploy to Azure Container Apps
**Then** backend `Dockerfile` uses `mcr.microsoft.com/dotnet/aspnet:10.0` base image
**And** frontend `Dockerfile` uses multi-stage build (node:20 build → nginx:alpine runtime)
**And** Azure Container Apps environment is created with `az containerapp env create`
**And** backend container app is created with external ingress on port 8080
**And** frontend container app is created with external ingress on port 80
**And** environment variables are configured (CosmosDb, BlobStorage, ApplicationInsights connection strings)
**And** managed identity is configured for Key Vault access
**And** both containers are accessible via HTTPS with managed certificates
**And** auto-scaling is configured (backend: min 1, max 10; frontend: min 1, max 3)

## Epic 2: Conversational Interface Foundation

Employees can interact with HRAgent through natural conversation on any device, establishing the core chat experience. Users can access HRAgent via web chat from desktop or mobile browser, have natural conversations with context maintained across messages, see real-time streaming AI responses, and receive clear feedback for all actions.

### Story 2.1: Create Conversation State Collections in MongoDB

As a developer,
I want to define conversation collections for MongoDB storage,
So that chat conversations can be persisted and retrieved by threadId.

**Acceptance Criteria:**

**Given** MongoDB connection is configured
**When** I create conversation models
**Then** `ConversationThread.cs` model is created with properties: Id (ObjectId), ThreadId, UserId, CreatedAt, Messages[]
**And** `Message.cs` model is created with properties: Id, Role (user/assistant), Text, CreatedAt, TokenCount
**And** Models use MongoDB attributes: `[BsonId]`, `[BsonElement]`, `[BsonRepresentation]`
**And** `ConversationRepository.cs` uses `IMongoCollection<ConversationThread>`
**And** ThreadId is indexed for fast lookups using `Builders<T>.IndexKeys.Ascending()`
**And** Models use PascalCase in C# but serialize to camelCase BSON automatically
**And** Conversation can be created and retrieved by threadId successfully using MongoDB.Driver LINQ

### Story 2.2: Implement AG-UI Server Endpoint with Microsoft Agent Framework

As a developer,
I want to register the AG-UI endpoint using Microsoft Agent Framework,
So that frontend can connect via SSE for real-time AI conversations.

**Acceptance Criteria:**

**Given** Microsoft.Agents.AI packages are installed
**When** I implement AG-UI endpoint in `Program.cs`
**Then** `Microsoft.Agents.AI` and `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` packages are installed
**And** `AgentService.cs` is created with `CreateAgent()` method initializing Agent Framework
**And** `Program.cs` registers agent as singleton: `builder.Services.AddSingleton<AgentService>()`
**And** AG-UI endpoint is mapped: `app.MapAGUI("/", agent)` using Minimal APIs pattern
**And** agent is configured with Azure OpenAI client from AI Foundry managed endpoint
**And** SSE connection can be established at `/` endpoint
**And** test message triggers agent response stream via SSE MESSAGE events

### Story 2.3: Build Basic Chat Interface Component (React)

As a developer,
I want to create the foundational chat UI component,
So that users can see messages and interact with the conversation.

**Acceptance Criteria:**

**Given** frontend React project is initialized
**When** I create `ChatInterface.tsx` component
**Then** component renders chat container with message list and input area
**And** `MessageBubble.tsx` component displays individual messages (user vs assistant styling)
**And** messages are styled mobile-first with ≥44px touch targets for buttons
**And** responsive layout works from 320px minimum width to desktop
**And** component uses Tailwind CSS with shadcn/ui Button and Card components
**And** chat interface fills viewport height appropriately on mobile and desktop
**And** scrolling works smoothly with auto-scroll to latest message

### Story 2.4: Integrate CopilotKit for AG-UI Client

As a developer,
I want to integrate CopilotKit to handle AG-UI protocol communication,
So that frontend can send/receive messages via SSE streaming.

**Acceptance Criteria:**

**Given** chat interface component exists
**When** I integrate CopilotKit library
**Then** `@copilotkit/react-core` and `@copilotkit/react-ui` packages are installed
**And** `main.tsx` wraps App with `<CopilotKit>` provider configured with backend URL
**And** `ChatInterface.tsx` uses CopilotKit hooks for message sending
**And** SSE connection is established automatically when component mounts
**And** user messages are sent to backend AG-UI endpoint
**And** assistant responses stream in real-time via SSE MESSAGE events
**And** connection auto-reconnects on network disruption with exponential backoff

### Story 2.5: Implement Conversation State Management (Zustand)

As a developer,
I want to manage conversation state in a Zustand store,
So that messages, streaming status, and threadId are tracked client-side.

**Acceptance Criteria:**

**Given** CopilotKit integration exists
**When** I create `conversationStore.ts`
**Then** Zustand store is created with state: threadId, messages[], isStreaming
**And** store includes actions: setThreadId(), addMessage(), setStreaming(), clearConversation()
**And** store is configured with devtools middleware for debugging
**And** `ChatInterface.tsx` subscribes to store selectively (only needed state slices)
**And** messages array updates trigger re-render of message list only
**And** isStreaming state controls display of typing indicator
**And** store state persists correctly across component re-mounts

### Story 2.6: Add Natural Language Date Parsing

As a developer,
I want the system to understand natural date expressions,
So that users can type "next Friday" or "March 10-12" without using date pickers.

**Acceptance Criteria:**

**Given** agent service is configured
**When** I implement date parsing in agent prompts
**Then** agent system prompt includes instructions for parsing natural dates
**And** agent correctly interprets "next Friday" as the upcoming Friday date
**And** agent correctly interprets "March 10-12" as March 10, 11, 12 date range
**And** agent handles relative dates: "tomorrow", "next week", "in 2 weeks"
**And** agent detects and skips weekends for workday contexts
**And** agent confirms parsed dates back to user for verification
**And** date parsing accuracy is ≥90% for common expressions

### Story 2.7: Add Browser Notifications Support

As a user,
I want to receive browser notifications for important updates,
So that I don't miss reminders or approval requests.

**Acceptance Criteria:**

**Given** chat interface is open in browser
**When** I enable browser notifications
**Then** notification permission is requested on first use
**And** user can enable/disable notifications in UI settings
**And** notifications appear for: reminder alerts, approval requests, PTO approvals
**And** notifications include actionable text (e.g., "Timesheet due Friday")
**And** clicking notification focuses browser tab and scrolls to relevant message
**And** notifications respect browser Do Not Disturb settings
**And** notification preferences persist in localStorage

### Story 2.8: Implement Streaming Indicators and Feedback

As a user,
I want clear visual feedback during AI response streaming,
So that I know the system is processing my request.

**Acceptance Criteria:**

**Given** user sends a message
**When** AI response is streaming
**Then** `StreamingIndicator.tsx` component displays animated typing indicator
**And** time-to-first-token is displayed when <1 second for engagement
**And** streaming text appears word-by-word with smooth animation
**And** user can see partial response while streaming continues
**And** indicator disappears when streaming completes
**And** error state is displayed if streaming fails with recovery options
**And** optimistic UI shows user message immediately (<300ms perceived feedback)

## Epic 3: PTO Management

Employees can request PTO conversationally, managers can approve with complete context, all synced with Factorial HR. Users can submit PTO requests via natural language ("I need March 10-12 off"), check PTO balance and request status, cancel pending requests. Managers receive approval notifications with context-rich cards and can approve/deny with one-click. All PTO data synchronized with Factorial HR in real-time.

### Story 3.1: Implement Factorial HR PTO Balance Retrieval

As an employee,
I want to check my PTO balance via conversation,
So that I know how many days I have available before requesting time off.

**Acceptance Criteria:**

**Given** FactorialClient is configured with Polly
**When** I ask "What's my PTO balance?"
**Then** agent calls `FactorialClient.GetPTOBalanceAsync(userId)` method
**And** method makes GET request to `/api/v1/employees/{userId}/time_off/balance`
**And** response is deserialized to `PTOBalanceResponse` with properties: Total, Used, Available
**And** agent responds with natural language: "You have 11 days of PTO available (15 total, 4 used)"
**And** Polly retry handles transient Factorial API failures (429, 500, 503)
**And** error is gracefully handled with fallback message if Factorial unavailable
**And** PTO balance call logs to Application Insights with duration and success/failure

### Story 3.2: Implement Conversational PTO Request Submission

As an employee,
I want to submit PTO requests using natural language,
So that I can request time off without navigating forms.

**Acceptance Criteria:**

**Given** user is authenticated and PTO balance endpoint works
**When** I type "I need March 10-12 off for family trip"
**Then** agent extracts: dates (March 10-12), reason (family trip)
**And** agent displays confirmation card showing: dates, reason, days requested (3), remaining balance after
**And** user confirms with one-tap "Submit" button
**And** backend creates PTO request in Factorial via `FactorialClient.SubmitPTORequestAsync()`
**And** request is validated against employee balance (sufficient days available)
**And** request is validated against company policy (notice period, blackout dates)
**And** success message confirms submission with request ID
**And** conversation is logged to Cosmos DB with threadId

### Story 3.3: Create PTO State Management Store (Frontend)

As a developer,
I want a Zustand store to manage PTO data client-side,
So that balance and requests are cached and easily accessible.

**Acceptance Criteria:**

**Given** conversationStore pattern is established
**When** I create `ptoStore.ts`
**Then** store manages state: balance, requests[], isLoading, error
**And** store includes actions: fetchBalance(), submitRequest(), cancelRequest()
**And** `fetchBalance()` calls backend `/pto/balance/{userId}` endpoint
**And** `submitRequest()` calls backend `/pto/request` POST endpoint
**And** store is configured with devtools middleware
**And** `PTOBalanceWidget.tsx` component subscribes to balance state
**And** loading states prevent duplicate API calls
**And** errors are captured and displayed with user-friendly messages

### Story 3.4: Build PTO Request Endpoints (Backend)

As a developer,
I want RESTful endpoints for PTO operations,
So that frontend can fetch balance and submit requests outside of chat.

**Acceptance Criteria:**

**Given** Minimal APIs pattern is established
**When** I create `PTOEndpoints.cs` in `/Endpoints` folder
**Then** endpoint group is created: `endpoints.MapGroup("/pto").RequireAuthorization()`
**And** GET `/pto/balance/{userId}` endpoint calls FactorialClient and returns balance
**And** POST `/pto/request` endpoint accepts `PTORequest` DTO (userId, startDate, endDate, reason)
**And** endpoint validates request against policy and balance before calling Factorial
**And** endpoint returns 200 with request ID on success
**And** endpoint returns 400 ValidationProblem if validation fails
**And** all operations log to AuditLogger with complete context
**And** endpoints are registered in `Program.cs` with `app.MapPTOEndpoints()`

### Story 3.5: Implement PTO Request Status Checking

As an employee,
I want to check the status of my pending PTO requests,
So that I know whether they've been approved or denied.

**Acceptance Criteria:**

**Given** user has submitted PTO requests
**When** I ask "What's the status of my PTO request?"
**Then** agent calls `FactorialClient.GetPTORequestsAsync(userId)`
**And** Factorial API returns list of requests with status (pending, approved, denied)
**And** agent displays requests in conversational format with status badges
**And** pending requests show "Awaiting approval from [Manager Name]"
**And** approved requests show approval date and approver name
**And** denied requests show denial reason and date
**And** user can click request ID to see full details

### Story 3.6: Implement PTO Request Cancellation

As an employee,
I want to cancel pending PTO requests,
So that I can change my plans without contacting HR.

**Acceptance Criteria:**

**Given** user has pending PTO request
**When** I say "Cancel my March 10-12 PTO request"
**Then** agent identifies the specific request by date range
**And** agent displays confirmation: "Cancel 3-day PTO request for March 10-12?"
**And** user confirms cancellation with one-tap button
**And** backend calls `FactorialClient.CancelPTORequestAsync(requestId)`
**And** request status changes to "Cancelled" in Factorial
**And** cancelled days are restored to employee balance
**And** manager is notified of cancellation if request was already approved
**And** cancellation logs to AuditLogger with reason and timestamp

### Story 3.7: Implement Policy Validation Rules

As a developer,
I want to validate PTO requests against company policies,
So that only valid requests are submitted to Factorial.

**Acceptance Criteria:**

**Given** PTO request is being submitted
**When** validation runs before Factorial submission
**Then** validation checks: sufficient PTO balance (requested <= available)
**And** validation checks: minimum notice period (e.g., 2 weeks for >3 days)
**And** validation checks: blackout dates (company holidays, critical project periods)
**And** validation checks: no overlapping approved PTO requests
**And** validation returns specific error message for each failed rule
**And** policy rules are configurable in `appsettings.json` (NoticeRequired, BlackoutDates)
**And** policy violations log to audit trail with rule and reason

### Story 3.8: Create Manager PTO Approval Notification System

As a manager,
I want to receive notifications when team members request PTO,
So that I can review and approve requests promptly.

**Acceptance Criteria:**

**Given** employee submits PTO request
**When** request is created in Factorial
**Then** backend identifies employee's manager from Factorial API
**And** notification is sent via browser push (if manager online) or email
**And** notification includes: employee name, dates, days, reason, remaining balance
**And** notification includes deep link to approval interface in HRAgent
**And** manager can click notification to open HRAgent approval view
**And** notification is marked as read when manager views request
**And** reminder notification sent if no action after 24 hours

### Story 3.9: Build Context-Rich Approval Cards (Frontend)

As a manager,
I want to see complete context for PTO approval decisions,
So that I can approve or deny with confidence.

**Acceptance Criteria:**

**Given** manager receives PTO approval notification
**When** I open the approval card in chat
**Then** `ApprovalCard.tsx` component displays rich context including:
  - Employee name, photo, role
  - Requested dates with day count
  - Current PTO balance and remaining after approval
  - Policy compliance status (Green ✓ if compliant)
  - Team coverage percentage during requested dates
  - Conflicting PTO requests from team members
  - Upcoming project milestones or deadlines
**And** card shows Green/Yellow/Red confidence indicator
**And** Green = "Safe to approve" with reasoning
**And** Yellow = "Review needed" with specific concerns
**And** Red = "Conflicts detected" with blocking issues
**And** one-click "Approve" and "Deny" buttons with optional comment field
**And** card is optimized for desktop with data visualization

### Story 3.10: Implement Manager Approval Actions

As a manager,
I want to approve or deny PTO requests with one click,
So that I can process approvals efficiently.

**Acceptance Criteria:**

**Given** approval card is displayed
**When** I click "Approve" or "Deny" button
**Then** POST request sent to `/approvals/{requestId}/approve` or `/deny` endpoint
**And** endpoint calls `FactorialClient.ApprovePTORequestAsync()` or `DenyPTORequestAsync()`
**And** approval status updates in Factorial HR immediately
**And** employee receives notification of approval/denial decision
**And** denial requires comment explaining reason
**And** approval decision logs to AuditLogger with complete reasoning chain
**And** optimistic UI updates approval card status immediately
**And** approval card shows "Approved ✓" or "Denied" badge
**And** manager can undo decision within 2-hour window if auto-approval feature enabled later

### Story 3.11: Implement Real-Time Factorial Sync

As a user,
I want PTO data to stay synchronized with Factorial HR,
So that HRAgent and Factorial always show the same information.

**Acceptance Criteria:**

**Given** PTO operations occur in HRAgent
**When** any PTO action is performed (submit, approve, deny, cancel)
**Then** data is written to Factorial API within 5 seconds
**And** background sync job reconciles HRAgent cache with Factorial every 15 minutes
**And** sync detects external changes (PTO approved directly in Factorial portal)
**And** sync updates HRAgent conversation store with latest status
**And** user is notified if external changes affect their conversations
**And** sync handles Factorial API rate limits with request queuing
**And** failed syncs are retried with exponential backoff up to 5 attempts
**And** permanent sync failures alert admin via Application Insights

### Story 3.12: Add PTO Request Notifications to Employees

As an employee,
I want to receive notifications when my PTO requests are approved or denied,
So that I know the outcome without checking repeatedly.

**Acceptance Criteria:**

**Given** manager processes PTO approval
**When** request status changes to approved or denied
**Then** browser notification is sent if employee is online
**And** email notification is sent as fallback if browser unavailable
**And** notification includes: decision (approved/denied), dates, manager name, optional comment
**And** notification deep links to conversation thread with full approval context
**And** approved notification includes confirmation: "Your PTO for March 10-12 has been approved"
**And** denied notification includes reason from manager comment
**And** notification preferences are configurable per user (browser, email, both, none)

## Epic 4: Timesheet Management

Employees can log work hours conversationally with project allocation, submit weekly timesheets, and receive helpful reminders. Users can log hours via conversation ("Log 8 hours today on Marketing Campaign"), specify project/task allocation, submit weekly timesheets with confirmation, check submission status and missing entries, receive reminder notifications before deadlines. All timesheet data synchronized with Factorial HR.

### Story 4.1: Implement Factorial HR Timesheet Retrieval

As an employee,
I want to view my timesheet entries via conversation,
So that I can see what hours I've already logged.

**Acceptance Criteria:**

**Given** FactorialClient is configured
**When** I ask "Show my timesheet for this week"
**Then** agent calls `FactorialClient.GetTimesheetsAsync(userId, startDate, endDate)`
**And** method makes GET request to `/api/v1/employees/{userId}/timesheet?start={date}&end={date}`
**And** response includes entries with: date, hours, project, task, status (draft, submitted)
**And** agent displays entries in conversational format grouped by date
**And** total hours for the week are calculated and displayed
**And** missing entries (workdays with 0 hours) are highlighted
**And** Polly retry handles Factorial API failures gracefully

### Story 4.2: Implement Conversational Hour Logging

As an employee,
I want to log work hours using natural language,
So that I can quickly record time without forms.

**Acceptance Criteria:**

**Given** user is authenticated
**When** I type "Log 8 hours today on Marketing Campaign"
**Then** agent extracts: hours (8), date (today), project (Marketing Campaign)
**And** agent confirms parsed information: "Log 8 hours on December 25, 2025 for Marketing Campaign?"
**And** user confirms with one-tap button
**And** backend calls `FactorialClient.SubmitTimesheetEntryAsync()` to create entry
**And** entry is created with status "draft" (not yet submitted)
**And** success confirmation shows: "Logged 8 hours ✓"
**And** agent handles multiple projects: "Log 6 hours on Marketing and 2 hours on Budget Review"
**And** entry logs to AuditLogger with complete context

### Story 4.3: Create Timesheet State Management Store (Frontend)

As a developer,
I want a Zustand store to manage timesheet data client-side,
So that entries and submission status are cached and accessible.

**Acceptance Criteria:**

**Given** store pattern is established
**When** I create `timesheetStore.ts`
**Then** store manages state: entries[], selectedDate, isSubmitting, totalHours
**And** store includes actions: fetchEntries(), logHours(), submitWeek(), checkStatus()
**And** `fetchEntries()` calls backend `/timesheets/{userId}?date={date}` endpoint
**And** `logHours()` calls backend `/timesheets` POST endpoint
**And** `submitWeek()` marks all draft entries as submitted
**And** store calculates totalHours automatically from entries array
**And** `TimesheetView.tsx` component subscribes to entries and totalHours
**And** store is configured with devtools middleware

### Story 4.4: Build Timesheet Endpoints (Backend)

As a developer,
I want RESTful endpoints for timesheet operations,
So that frontend can manage timesheets outside of chat.

**Acceptance Criteria:**

**Given** Minimal APIs pattern is established
**When** I create `TimesheetEndpoints.cs` in `/Endpoints` folder
**Then** endpoint group is created: `endpoints.MapGroup("/timesheets").RequireAuthorization()`
**And** GET `/timesheets/{userId}?date={date}` endpoint fetches entries for specified week
**And** POST `/timesheets` endpoint accepts `TimesheetRequest` DTO (userId, date, hours, project, task)
**And** PUT `/timesheets/{id}` endpoint updates existing draft entry
**And** POST `/timesheets/submit` endpoint marks all draft entries as submitted for the week
**And** endpoints validate: hours ≤ 24 per day, date is not future, project exists
**And** all operations sync with Factorial HR via FactorialClient
**And** all operations log to AuditLogger
**And** endpoints are registered in `Program.cs`

### Story 4.5: Implement Project/Task Allocation

As an employee,
I want to specify which projects I worked on,
So that time is allocated correctly for billing and reporting.

**Acceptance Criteria:**

**Given** user is logging hours
**When** I specify project name in conversation
**Then** agent attempts to match project name to Factorial project list
**And** if exact match found, project is used directly
**And** if fuzzy match found, agent asks: "Did you mean 'Marketing Campaign 2025'?"
**And** if no match, agent asks user to clarify or lists available projects
**And** agent handles task allocation: "Log 8 hours on Marketing Campaign - Social Media task"
**And** timesheet entry includes: projectId, projectName, taskId (optional), taskName (optional)
**And** project allocation is validated against user's assigned projects in Factorial
**And** error message displayed if user not assigned to specified project

### Story 4.6: Implement Weekly Timesheet Submission

As an employee,
I want to submit my complete weekly timesheet,
So that my hours are finalized and approved.

**Acceptance Criteria:**

**Given** user has logged hours for the week
**When** I say "Submit my timesheet"
**Then** agent retrieves all draft entries for current week (Monday-Friday)
**And** agent displays weekly summary with total hours per day and per project
**And** agent validates: all workdays have entries, total hours are reasonable (30-50 hours/week)
**And** if validation passes, agent shows "Submit 40 hours for the week?" confirmation
**And** user confirms submission with one-tap button
**And** backend marks all entries as "submitted" in Factorial via batch API call
**And** submission logs to AuditLogger with complete week details
**And** success confirmation: "Timesheet submitted ✓ - 40 hours for Dec 23-27, 2025"
**And** submitted timesheets cannot be edited without manager approval

### Story 4.7: Implement Timesheet Status Checking

As an employee,
I want to check if I've submitted my timesheet,
So that I don't miss the weekly deadline.

**Acceptance Criteria:**

**Given** timesheet deadline is Friday EOD
**When** I ask "Did I submit my timesheet this week?"
**Then** agent checks submission status for current week
**And** if submitted, agent confirms: "Yes ✓ You submitted 40 hours on Friday, Dec 27"
**And** if not submitted, agent shows missing days: "Not yet. Missing entries for Thursday, Friday (12 hours)"
**And** if partially complete, agent shows: "You have 24 hours logged but not submitted yet"
**And** agent provides quick action: "Would you like to submit now?" with button
**And** status includes last modification date for tracking
**And** agent can check previous weeks: "Did I submit last week's timesheet?"

### Story 4.8: Implement Missing Entry Detection

As an employee,
I want to be notified of missing timesheet entries,
So that I can complete my timesheet before the deadline.

**Acceptance Criteria:**

**Given** timesheet has gaps for the current week
**When** I open HRAgent on Wednesday-Friday
**Then** agent proactively mentions: "Reminder: You haven't logged hours for Monday and Tuesday"
**And** missing entry detection excludes weekends and company holidays
**And** detection excludes PTO days (cross-references approved PTO requests)
**And** detection excludes sick days or other leave types
**And** agent offers quick fill: "Would you like to log hours for those days now?"
**And** missing entries are highlighted in weekly timesheet view
**And** count of missing entries shown in status check

### Story 4.9: Implement Reminder Notifications Before Deadline

As an employee,
I want to receive reminders before the timesheet deadline,
So that I don't forget to submit.

**Acceptance Criteria:**

**Given** timesheet deadline is Friday 6:00 PM
**When** reminder trigger conditions are met
**Then** first reminder sent on Wednesday afternoon if 0 hours logged
**And** second reminder sent on Thursday afternoon if incomplete (<32 hours logged)
**And** final reminder sent on Friday 3:00 PM if not submitted
**And** reminder notification includes: hours logged so far, missing days, deadline time
**And** reminder deep links to HRAgent chat with pre-filled "Submit timesheet" context
**And** reminder timing respects user's calendar (avoids meeting times)
**And** user can snooze reminder for 2 hours
**And** reminder preferences are configurable (frequency, timing, channels)

### Story 4.10: Create User Pattern Recognition Storage

As a developer,
I want to store user timesheet patterns in MongoDB,
So that pattern-based features like "log like last week" can be implemented later.

**Acceptance Criteria:**

**Given** MongoDB connection is configured
**When** I create pattern recognition collections
**Then** `UserPattern.cs` model is created with properties: Id (ObjectId), UserId, TimesheetDayOfWeek, PreferredSubmissionTime, ProjectAllocations[]
**And** `UserPatternRepository.cs` uses `IMongoCollection<UserPattern>` with "user-patterns" collection
**And** UserId is indexed for fast lookups using `Builders<T>.IndexKeys.Ascending(x => x.UserId)`
**And** `PatternService.cs` is created to analyze and store patterns
**And** service has method: `AnalyzeTimesheetPatterns(userId)` that runs weekly
**And** patterns include: typical submission day (e.g., Friday), typical hours per project
**And** patterns are updated incrementally as more data is collected using MongoDB upsert operations
**And** pattern data will enable "log like last week" feature in Growth phase

### Story 4.11: Implement Real-Time Factorial Timesheet Sync

As a user,
I want timesheet data to stay synchronized with Factorial HR,
So that both systems always show the same information.

**Acceptance Criteria:**

**Given** timesheet operations occur in HRAgent
**When** any timesheet action is performed (log, edit, submit)
**Then** data is written to Factorial API within 5 seconds
**And** background sync job reconciles every 30 minutes
**And** sync detects external changes (hours logged directly in Factorial portal)
**And** sync updates HRAgent timesheet store with latest entries
**And** conflicting changes (edited in both systems) prompt user to choose version
**And** sync handles Factorial API rate limits with request queuing
**And** failed syncs retry with exponential backoff
**And** permanent sync failures alert admin via Application Insights

### Story 4.12: Build Timesheet View Component (Frontend)

As an employee,
I want a visual weekly timesheet view,
So that I can see all my hours at a glance.

**Acceptance Criteria:**

**Given** timesheet entries exist
**When** I view the timesheet interface
**Then** `TimesheetView.tsx` component displays weekly grid (Monday-Friday rows)
**And** each day shows: date, total hours, project breakdown
**And** missing days are highlighted with "Add hours" prompt
**And** submitted days show green checkmark and are read-only
**And** draft days have edit/delete buttons
**And** total hours for week displayed prominently at bottom
**And** "Submit Timesheet" button enabled when week is complete
**And** responsive design: condensed on mobile, expanded grid on desktop
**And** component subscribes to timesheetStore for reactive updates

## Epic 5: Policy Knowledge & Support (RAG)

Employees get instant, accurate answers to HR policy questions with source citations, reducing HR interruptions. Users can ask HR policy questions via conversation, receive accurate answers with source document citations, get escalated to HR admin for complex questions. HR admins can view and respond to escalated questions. System learns from frequently asked questions.

### Story 5.1: Set Up Knowledge Base Document Storage

As a developer,
I want to store HR policy documents for RAG retrieval,
So that the system can answer policy questions accurately.

**Acceptance Criteria:**

**Given** Azure AI Search or MongoDB vector search is available
**When** I configure knowledge base storage
**Then** policy documents are stored in searchable format (PDF, Markdown, or plaintext)
**And** document metadata includes: title, category (PTO, expenses, benefits), lastUpdated, source URL
**And** documents are chunked into ~500 token segments for optimal retrieval
**And** chunk overlap of 50 tokens maintains context across boundaries
**And** document ingestion pipeline converts PDFs to text and creates chunks
**And** knowledge base can be updated by HR admin via upload interface
**And** document versioning tracks changes with timestamp and author

### Story 5.2: Implement Document Embedding Pipeline

As a developer,
I want to generate embeddings for policy documents,
So that semantic search can find relevant content.

**Acceptance Criteria:**

**Given** knowledge base documents are stored
**When** I implement embedding generation
**Then** `Azure.AI.OpenAI` package is used for text-embedding-ada-002 model
**And** `KnowledgeBaseService.cs` is created with `EmbedDocumentsAsync()` method
**And** each document chunk is embedded using text-embedding-ada-002 (1536 dimensions)
**And** embeddings are stored alongside chunks in vector database
**And** embedding generation runs in background job when new documents are added
**And** batch embedding process handles rate limits (requests per minute quota)
**And** embedding generation logs to Application Insights with duration and token count
**And** failed embeddings retry with exponential backoff

### Story 5.3: Implement Semantic Search for Policy Questions

As a developer,
I want to search policy documents using semantic similarity,
So that relevant policy content is retrieved for user questions.

**Acceptance Criteria:**

**Given** policy documents are embedded
**When** user asks a policy question
**Then** `KnowledgeBaseService.SearchPoliciesAsync(query)` is called
**And** user question is embedded using text-embedding-ada-002
**And** cosine similarity search finds top 5 most relevant document chunks
**And** results are ranked by similarity score (0.0 to 1.0)
**And** chunks with score <0.7 are filtered out (low relevance)
**And** service returns: chunk text, source document title, page number, similarity score
**And** search completes in <500ms for responsive UX
**And** search results include metadata for source citation

### Story 5.4: Integrate RAG into Agent Responses

As an employee,
I want to ask HR policy questions and get accurate answers,
So that I don't have to search through policy documents manually.

**Acceptance Criteria:**

**Given** semantic search is working
**When** I ask "What's the PTO notice requirement?"
**Then** agent detects policy question intent
**And** agent calls `KnowledgeBaseService.SearchPoliciesAsync("PTO notice requirement")`
**And** retrieved policy chunks are injected into agent context
**And** agent synthesizes answer from retrieved chunks
**And** agent response includes natural language answer: "You need to give 2 weeks notice for PTO requests longer than 3 days"
**And** agent cites source: "(Source: Employee Handbook, Section 4.2)"
**And** if no relevant policy found (similarity <0.7), agent admits uncertainty
**And** agent offers escalation: "I'm not sure—would you like me to ask HR?"

### Story 5.5: Add Source Citations to Policy Answers

As an employee,
I want to see which policy document the answer came from,
So that I can verify the information if needed.

**Acceptance Criteria:**

**Given** agent answers policy question using RAG
**When** answer is displayed
**Then** response includes clickable source citation
**And** citation format: "(Source: [Document Title], Section X.Y)"
**And** clicking citation opens source document in new tab or embedded viewer
**And** if answer synthesizes multiple sources, all sources are cited
**And** citation includes page number or section identifier for quick reference
**And** source document URL or blob storage link is accessible
**And** citation appears at end of answer in clear, non-intrusive format

### Story 5.6: Implement Confidence Threshold and Escalation

As an employee,
I want the system to admit when it's uncertain,
So that I get escalated to HR instead of receiving inaccurate information.

**Acceptance Criteria:**

**Given** semantic search returns low-confidence results
**When** similarity score <0.7 for all retrieved chunks
**Then** agent responds: "I'm not certain about this. Let me escalate to HR for you."
**And** agent does NOT hallucinate or guess an answer
**And** escalation creates ticket in HR admin queue
**And** ticket includes: original question, user ID, timestamp, conversation threadId
**And** HR admin is notified via dashboard and email
**And** user receives confirmation: "I've sent your question to HR. You'll get a response within 24 hours."
**And** confidence threshold (0.7) is configurable in `appsettings.json`

### Story 5.7: Build HR Admin Escalation Dashboard

As an HR administrator,
I want to view and respond to escalated policy questions,
So that employees get accurate answers for complex cases.

**Acceptance Criteria:**

**Given** policy questions are escalated
**When** I access HR admin dashboard
**Then** dashboard displays escalation queue with: question text, employee name, date, status
**And** status values: pending, in-progress, resolved
**And** I can filter by status and search by keyword
**And** clicking escalation shows full conversation context from HRAgent
**And** I can respond directly in dashboard with answer
**And** response is delivered to employee via HRAgent conversation thread
**And** resolved escalations are archived with Q&A for future knowledge base improvement
**And** dashboard shows escalation metrics: total count, average response time, resolution rate

### Story 5.8: Implement FAQ Tracking and Analytics

As an HR administrator,
I want to see which policy questions are asked most frequently,
So that I can improve documentation and reduce future escalations.

**Acceptance Criteria:**

**Given** policy questions are being asked via HRAgent
**When** I access FAQ analytics dashboard
**Then** dashboard shows top 20 most frequently asked questions with count
**And** questions are grouped by topic category (PTO, expenses, benefits, payroll)
**And** dashboard shows trend over time (questions per week)
**And** questions with high escalation rate are highlighted (indicates documentation gap)
**And** I can export FAQ data to CSV for reporting
**And** I can mark questions for knowledge base improvement
**And** system suggests creating new policy documents for frequently escalated topics

### Story 5.9: Add Policy Update Notifications

As an HR administrator,
I want to notify employees when policies change,
So that everyone is aware of important updates.

**Acceptance Criteria:**

**Given** policy document is updated in knowledge base
**When** I mark update as "notify employees"
**Then** notification is sent to all employees via HRAgent
**And** notification appears in chat as system message: "Policy Update: PTO notice requirement changed"
**And** notification includes summary of change and link to full policy
**And** employees can dismiss notification after reading
**And** notification persists in chat history for reference
**And** urgent policy changes can be marked as "acknowledgment required"
**And** dashboard tracks acknowledgment rate for critical policy updates

### Story 5.10: Implement Knowledge Base Admin Interface

As an HR administrator,
I want to upload and manage policy documents,
So that the knowledge base stays current.

**Acceptance Criteria:**

**Given** HR admin has access to admin dashboard
**When** I access knowledge base management section
**Then** I can upload new policy documents (PDF, DOCX, Markdown)
**And** documents are automatically processed: text extraction, chunking, embedding
**And** I can edit document metadata: title, category, effective date
**And** I can mark documents as archived (excluded from search but retained for history)
**And** I can preview how documents are chunked before publishing
**And** document upload shows progress: extracting text, generating embeddings, indexing
**And** I can search existing documents and view embedding quality metrics
**And** dashboard shows: total documents, total chunks, last update date, storage size

## Epic 6: Audit, Compliance & Monitoring

Complete compliance infrastructure with immutable audit trails, real-time monitoring, and analytics dashboards for HR administrators. All transactions automatically generate immutable audit logs. Complete decision traceability months later (who, what, when, why, reasoning). HR admins access audit trails for compliance reporting, view usage analytics and performance dashboards. Policy violations automatically flagged. GDPR-compliant data handling with export capability.

### Story 6.1: Implement Immutable Audit Log Infrastructure

As a developer,
I want all transactions to generate immutable audit log entries,
So that complete compliance records are maintained.

**Acceptance Criteria:**

**Given** AuditLogger service is configured with Blob Storage
**When** any write operation occurs (PTO request, timesheet submit, approval decision)
**Then** `AuditLogger.LogAsync(eventType, userId, data)` is called before returning response
**And** log entry is written to append blob in format: `audit/{year}/{month}/{day}/{threadId}.jsonl`
**And** entry includes: timestamp (UTC), userId, eventType, data (JSON), reasoning, requestId
**And** blob storage container has 7-year immutability policy configured
**And** append blobs prevent overwrites (append-only, no deletes)
**And** log writes are thread-safe using SemaphoreSlim for concurrent operations
**And** failed log writes are retried 3 times, then alert admin if still failing
**And** audit log write completes in <100ms to avoid blocking user operations

### Story 6.2: Implement Complete Decision Context Capture

As a compliance officer,
I want every decision to include complete context,
So that I can understand reasoning months later during audits.

**Acceptance Criteria:**

**Given** manager approves PTO request
**When** approval is logged to audit trail
**Then** log entry captures: who (manager userId), what (approval decision), when (timestamp)
**And** log captures why: approval reasoning, risk factors, policy validation results
**And** log captures context: employee balance, team coverage, conflicting requests, project deadlines
**And** log captures AI involvement: agent confidence score, data sources consulted, alternative recommendations
**And** log includes conversation threadId for full chat history reference
**And** data structure is JSON for easy querying and analysis
**And** all personally identifiable information (PII) is marked for GDPR compliance

### Story 6.3: Build Audit Trail Query Interface for HR Admins

As an HR administrator,
I want to search and filter audit logs,
So that I can generate compliance reports and investigate issues.

**Acceptance Criteria:**

**Given** audit logs are stored in Blob Storage
**When** I access audit trail interface in admin dashboard
**Then** I can filter by: date range, userId, eventType, requestId
**And** search supports keyword search across all log fields
**And** results display: timestamp, user, event type, summary, view details button
**And** clicking details shows complete JSON log entry with formatting
**And** I can export filtered results to CSV or JSON for external reporting
**And** export includes all fields with option to redact PII
**And** query executes in <3 seconds for 1 month of data
**And** pagination limits results to 100 per page for performance

### Story 6.4: Implement Policy Violation Detection and Flagging

As an HR administrator,
I want policy violations to be automatically detected and flagged,
So that I can review and address compliance issues proactively.

**Acceptance Criteria:**

**Given** policy validation runs on PTO and timesheet operations
**When** validation detects rule violation
**Then** violation is logged to audit trail with severity: warning, error, critical
**And** warning violations allow operation but flag for review (e.g., short notice PTO)
**And** error violations block operation and require HR override (e.g., insufficient balance)
**And** critical violations block operation and alert admin immediately (e.g., fraud detection)
**And** admin dashboard shows policy violation queue with: type, user, date, status
**And** violations can be reviewed and marked as: justified, requires action, false positive
**And** violation trends are tracked for policy improvement insights
**And** alerts sent via email and dashboard for critical violations within 5 minutes

### Story 6.5: Implement GDPR-Compliant Data Handling

As a data protection officer,
I want the system to comply with GDPR requirements,
So that employee data is handled lawfully.

**Acceptance Criteria:**

**Given** system stores employee personal data
**When** GDPR compliance features are implemented
**Then** data retention policies are enforced: audit logs (7 years), conversations (1 year), patterns (2 years)
**And** DELETE `/conversations/{threadId}` endpoint permanently removes conversation data
**And** DELETE `/user/{userId}/data` endpoint removes all user data (right to be forgotten)
**And** data export endpoint provides complete user data in machine-readable format (JSON)
**And** data export includes: conversations, PTO history, timesheet records, audit logs
**And** consent tracking logs user agreement to data processing with timestamp
**And** PII is identified and encrypted at rest using Azure-managed encryption
**And** data processing notice displayed on first use with consent checkbox
**And** user can withdraw consent and trigger data deletion via admin request

### Story 6.6: Build Usage Analytics Dashboard

As an HR administrator,
I want to see usage analytics,
So that I can track adoption and identify improvement opportunities.

**Acceptance Criteria:**

**Given** Application Insights is collecting telemetry
**When** I access usage analytics dashboard
**Then** dashboard shows: total users, daily active users (DAU), monthly active users (MAU)
**And** adoption metrics: % tried (used at least once), % adopted (used 3+ times/week)
**And** feature usage breakdown: PTO requests, timesheet logs, policy questions, approvals
**And** usage trends over time (line chart by week)
**And** user segments: employees, managers, HR admins
**And** top power users (highest usage) and inactive users (no usage in 2+ weeks)
**And** all metrics are updated daily from Application Insights data
**And** metrics can be exported to CSV for executive reporting

### Story 6.7: Build Performance Metrics Dashboard

As a developer,
I want to monitor system performance in real-time,
So that I can detect and resolve issues quickly.

**Acceptance Criteria:**

**Given** Application Insights is collecting performance data
**When** I access performance dashboard
**Then** dashboard shows: API latency p50/p95/p99, error rate, success rate
**And** SSE streaming metrics: time-to-first-token p50/p95, connection success rate
**And** dependency metrics: Cosmos DB latency, Factorial API success rate, Blob Storage latency
**And** LLM metrics: token consumption per conversation, average tokens per request, cost per conversation
**And** auto-scaling metrics: current replica count, CPU %, memory %, scale events
**And** alerts configured for: p95 latency >3s, error rate >1%, Factorial failures >2%
**And** dashboard auto-refreshes every 30 seconds
**And** drill-down views show detailed traces for slow requests via Application Insights

### Story 6.8: Build Satisfaction Metrics Tracking

As an HR administrator,
I want to track user satisfaction,
So that I can measure if HRAgent is delivering value.

**Acceptance Criteria:**

**Given** users interact with HRAgent
**When** satisfaction tracking is implemented
**Then** pulse survey prompt appears weekly: "How helpful was HRAgent this week?" (1-5 stars)
**And** survey includes optional comment field for feedback
**And** survey is non-intrusive: dismissible, appears only once per week
**And** satisfaction dashboard shows: average rating over time, rating distribution, trend
**And** dashboard displays recent feedback comments with sentiment analysis
**And** low ratings (<3 stars) are flagged for HR team review
**And** satisfaction targets are tracked: ≥4.0 average rating goal
**And** feedback is correlated with feature usage to identify pain points

### Story 6.9: Implement Time Savings Calculation and Reporting

As an HR administrator,
I want to calculate time savings from HRAgent usage,
So that I can demonstrate ROI to executives.

**Acceptance Criteria:**

**Given** users perform PTO and timesheet operations via HRAgent
**When** time savings are calculated
**Then** baseline time per operation is configured: PTO request (15 min), timesheet entry (20 min), approval (30 min)
**And** HRAgent time per operation is tracked via Application Insights: PTO (2 min), timesheet (1 min), approval (2 min)
**And** time savings = (baseline - actual) × operation count
**And** dashboard shows: total hours saved per week, per month, per user
**And** time savings monetized using average hourly wage ($50/hour default, configurable)
**And** ROI calculation: (time savings value - infrastructure cost) / infrastructure cost × 100
**And** executive report generated monthly with: total savings, ROI %, user adoption, satisfaction
**And** savings breakdown by category: PTO, timesheets, approvals, policy questions

### Story 6.10: Build Exception Queue Management Interface

As an HR administrator,
I want to manage exceptions and edge cases,
So that I can resolve issues that require human intervention.

**Acceptance Criteria:**

**Given** system encounters exceptions (policy conflicts, validation failures, escalations)
**When** I access exception queue in admin dashboard
**Then** queue displays all pending exceptions with: type, user, date, priority, status
**And** exception types: policy override requests, conflicting data, failed syncs, escalated questions
**And** I can filter by type and priority (low, medium, high, critical)
**And** clicking exception shows complete context: user request, system reasoning, conflict details
**And** I can take actions: approve override, deny with reason, resolve conflict, reassign
**And** actions log to audit trail with admin userId and reasoning
**And** resolved exceptions move to history with resolution note
**And** exception metrics tracked: total count, average resolution time, resolution rate
**And** high-priority exceptions send email alerts to HR team within 15 minutes

### Story 6.11: Implement Configuration Management Interface

As an HR administrator,
I want to configure system policies and thresholds,
So that HRAgent adapts to company-specific rules.

**Acceptance Criteria:**

**Given** system has configurable policies
**When** I access configuration interface in admin dashboard
**Then** I can configure PTO policies: notice required (days), blackout dates, maximum consecutive days
**And** I can configure timesheet policies: minimum hours/day, maximum hours/day, deadline day of week
**And** I can configure approval policies: auto-approval thresholds, escalation rules, notification timing
**And** I can configure reminder policies: timing preferences, frequency, channels (email, browser, both)
**And** I can configure AI parameters: confidence thresholds, RAG similarity cutoff, token limits
**And** configuration changes are versioned with timestamp and admin who made change
**And** configuration changes take effect immediately without requiring deployment
**And** configuration export/import supported for backup and cross-environment consistency

## Epic 7: Manager Approval Workflows

Managers efficiently process approval requests with complete context assembly and intelligent recommendations. Managers receive approval requests with all decision context assembled, see team coverage visualization, policy compliance, risk scoring. Can approve or deny with one-click and optional comments. Requestors get notified of approval decisions. All approval decisions captured in audit trail with reasoning.

### Story 7.1: Implement Approval Request Routing

As a manager,
I want to receive approval requests for my team members only,
So that I can focus on requests I'm responsible for.

**Acceptance Criteria:**

**Given** employee submits PTO or timesheet correction request
**When** request requires approval
**Then** system identifies employee's manager from Factorial HR organizational hierarchy
**And** approval notification is routed to manager's HRAgent instance
**And** approval appears in manager's conversation thread and approval queue
**And** if employee has multiple managers (matrix org), all managers are notified
**And** approval delegation is supported: manager can delegate to another manager temporarily
**And** delegated approvals route to delegate with note: "Delegated by [Manager Name]"
**And** routing failures (no manager found) escalate to HR admin queue
**And** routing logs to audit trail with manager identification reasoning

### Story 7.2: Implement Context Assembly from Multiple Systems

As a developer,
I want to gather all relevant context for approval decisions,
So that managers have complete information in one view.

**Acceptance Criteria:**

**Given** approval request is routed to manager
**When** context assembly runs
**Then** system retrieves: employee data (name, role, photo) from Factorial
**And** system retrieves: PTO balance (total, used, remaining) from Factorial
**And** system retrieves: policy compliance status (notice period, blackout dates) from validation service
**And** system retrieves: team calendar to calculate coverage percentage during requested dates
**And** system retrieves: conflicting PTO requests from other team members
**And** system retrieves: upcoming project milestones from project management integration
**And** system retrieves: historical approval patterns (past decisions for similar requests)
**And** all data is assembled in <2 seconds for responsive UX
**And** context assembly failures gracefully degrade (show partial context, flag missing data)

### Story 7.3: Build Desktop-Rich Approval Card Component

As a manager,
I want to see complete approval context in a rich visual card,
So that I can make informed decisions quickly.

**Acceptance Criteria:**

**Given** approval request is received
**When** I view approval card in desktop interface
**Then** `ApprovalCard.tsx` component displays in multi-column layout (optimized for ≥1024px width)
**And** left column shows: employee photo, name, role, request details (dates, reason, days count)
**And** center column shows: team coverage visualization (bar chart), conflicting requests, milestone timeline
**And** right column shows: policy compliance checklist, PTO balance gauge, historical context
**And** top of card shows confidence indicator: Green ✓ "Safe to approve", Yellow ⚠ "Review needed", Red ✗ "Conflicts detected"
**And** confidence indicator includes transparent reasoning tooltip
**And** one-click "Approve" and "Deny" buttons are prominently placed with clear CTAs
**And** optional comment field for approval/denial reasoning
**And** card is styled with shadcn/ui components and Tailwind CSS
**And** data visualization uses Chart.js or Recharts for coverage/balance gauges

### Story 7.4: Implement Green/Yellow/Red Confidence Signals

As a manager,
I want to see confidence indicators with transparent reasoning,
So that I understand why the system recommends approval or caution.

**Acceptance Criteria:**

**Given** context assembly is complete
**When** confidence scoring algorithm runs
**Then** Green (safe to approve) is shown when: policy compliant, coverage ≥70%, no conflicts, balance sufficient
**And** Yellow (review needed) is shown when: short notice, coverage 50-69%, minor policy concern, balance borderline
**And** Red (conflicts detected) is shown when: policy violation, coverage <50%, conflicting approvals, insufficient balance
**And** reasoning is displayed in natural language: "Coverage is 75% (Sarah, Mike available), no conflicts, policy compliant"
**And** each risk factor has tooltip with details: hover "coverage 75%" shows team member availability details
**And** confidence score is calculated from weighted factors: policy (30%), coverage (30%), conflicts (25%), balance (15%)
**And** weights are configurable in admin dashboard
**And** confidence algorithm is explainable in audit trail (shows factor weights and scores)

### Story 7.5: Implement One-Click Approval Actions

As a manager,
I want to approve or deny with one click,
So that I can process approvals efficiently.

**Acceptance Criteria:**

**Given** approval card is displayed
**When** I click "Approve" or "Deny" button
**Then** POST request sent to `/approvals/{requestId}/approve` or `/deny` endpoint immediately
**And** optimistic UI updates card status to "Approved ✓" or "Denied" while request processes
**And** backend calls `FactorialClient.ApproveRequestAsync()` or `DenyRequestAsync()`
**And** Factorial status updates within 5 seconds
**And** employee receives notification of decision immediately after Factorial update
**And** approval decision logs to AuditLogger with: manager userId, decision, reasoning, context snapshot
**And** denial requires comment explaining reason (enforced via validation)
**And** approval card is removed from queue and moves to history section
**And** if Factorial API call fails, UI reverts optimistic update and shows error with retry option

### Story 7.6: Implement Approval Comment Handling

As a manager,
I want to add comments to approval decisions,
So that employees understand my reasoning.

**Acceptance Criteria:**

**Given** approval card is displayed
**When** I add comment before approving/denying
**Then** optional comment field accepts up to 500 characters
**And** comment is included in employee notification
**And** comment is logged to audit trail with decision
**And** denial REQUIRES comment (enforced via validation, button disabled until comment added)
**And** approval comment is optional but encouraged with placeholder: "Optional: Add note for employee"
**And** comment supports mentions: "@employee please reschedule sprint planning"
**And** common comment templates available: "Approved - have a great trip!", "Coverage concern resolved"
**And** character count shows remaining characters as user types

### Story 7.7: Implement Team Coverage Visualization

As a manager,
I want to see team coverage during requested dates,
So that I can ensure adequate staffing.

**Acceptance Criteria:**

**Given** approval card is displayed
**When** team coverage is calculated
**Then** system retrieves: all team members, their PTO schedules, project assignments
**And** coverage percentage calculated: (available team members / total team members) × 100
**And** visualization shows: coverage bar (green ≥70%, yellow 50-69%, red <50%)
**And** visualization lists: available members (green checkmark), on PTO (calendar icon), unassigned (gray)
**And** clicking team member shows their schedule and project load
**And** coverage accounts for part-time employees (weighted by FTE)
**And** coverage highlights skill gaps: "3 developers available, 0 designers available"
**And** historical coverage comparison shown: "Typical coverage this time of year: 80%"

### Story 7.8: Implement Conflicting Request Detection

As a manager,
I want to see if other team members have conflicting PTO,
So that I can avoid scheduling issues.

**Acceptance Criteria:**

**Given** approval request is for specific dates
**When** conflict detection runs
**Then** system retrieves all approved and pending PTO requests for team during same dates
**And** conflicts are displayed in approval card with: employee name, dates, overlap days
**And** visual timeline shows: current request, conflicting requests, available dates
**And** conflicts are categorized: direct overlap (same dates), adjacent overlap (day before/after)
**And** system flags critical overlaps: "All senior developers unavailable March 10-12"
**And** system suggests alternatives: "No conflicts if employee takes March 13-15 instead"
**And** conflicts trigger Yellow or Red confidence signal depending on coverage impact

### Story 7.9: Implement Project Milestone Integration

As a manager,
I want to see upcoming project milestones,
So that I can avoid approving PTO during critical delivery periods.

**Acceptance Criteria:**

**Given** project management integration is configured
**When** approval card displays
**Then** system retrieves: project milestones within ±2 weeks of requested dates
**And** milestones are displayed with: project name, milestone name, deadline, risk level
**And** risk level is calculated: High (deadline during PTO, employee is critical path), Medium (deadline within 3 days of return), Low (no impact)
**And** approval card shows warning if High risk: "Sprint demo March 11 - Employee is presenting"
**And** system suggests mitigation: "Reschedule demo or reassign to [Team Member]"
**And** manager can view full project timeline in expanded view
**And** milestone data caches for 1 hour to reduce external API calls

### Story 7.10: Build Approval History and Analytics

As a manager,
I want to see my approval history and patterns,
So that I can maintain consistency in decisions.

**Acceptance Criteria:**

**Given** manager has processed approvals
**When** I access approval history section
**Then** history displays all past approval decisions with: date, employee, request type, decision, reasoning
**And** history is filterable by: decision (approved, denied), employee, date range, request type
**And** statistics shown: total approvals, approval rate, average response time, denial reasons
**And** patterns highlighted: "You typically approve <3 day requests immediately, review longer requests"
**And** inconsistency warnings: "This denial differs from previous similar approved requests"
**And** export to CSV for personal record keeping or performance reviews
**And** history links to full conversation context for each decision

### Story 7.11: Implement Approval Notification to Requestors

As an employee,
I want to be notified immediately when my request is approved or denied,
So that I can adjust plans accordingly.

**Acceptance Criteria:**

**Given** manager processes approval decision
**When** decision is finalized in Factorial
**Then** browser notification sent to employee if online: "[Manager Name] approved your PTO for March 10-12"
**And** email notification sent as fallback if browser unavailable
**And** notification includes: decision, dates, manager name, optional comment, request ID
**And** notification deep links to conversation thread with full approval context
**And** approved notification shows confirmation and next steps: "Approved ✓ Enjoy your time off!"
**And** denied notification shows reason and suggestions: "Denied - Coverage concern. Try March 13-15?"
**And** notification appears in HRAgent chat as system message with approval card summary
**And** employee can reply to notification with follow-up questions

### Story 7.12: Implement Approval Decision Audit Trail

As a compliance officer,
I want complete audit trails for all approval decisions,
So that I can trace reasoning months later.

**Acceptance Criteria:**

**Given** approval decision is made
**When** audit log is written
**Then** log captures: manager userId, employee userId, request type, decision, timestamp
**And** log captures reasoning: confidence score, risk factors, coverage percentage, policy status
**And** log captures context snapshot: team availability, conflicting requests, project milestones at decision time
**And** log captures AI involvement: recommendations made, alternative suggestions, data sources consulted
**And** log includes conversation threadId for full chat history reference
**And** log includes requestId for cross-referencing with Factorial records
**And** all approval decisions are queryable in admin audit interface
**And** audit trail is immutable (append-only Blob Storage with 7-year retention)
