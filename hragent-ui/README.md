# HRAgent Frontend

React + TypeScript + Vite frontend for the HRAgent application with Azure AD authentication.

## Technology Stack

- **Framework:** React 19.2 with TypeScript 5.9
- **Build Tool:** Vite 7.2
- **Authentication:** MSAL.js 4.27.0 (Microsoft Authentication Library)
- **State Management:** Zustand 5.0.9
- **Testing:** Vitest 4.0.16 + React Testing Library

## Prerequisites

- Node.js 20.x or later
- Azure AD tenant with registered Single Page Application (SPA)
- Backend API running (HRAgent.Api)

## Frontend Authentication Setup

This application uses **Microsoft Authentication Library (MSAL.js)** for Azure AD authentication with the Authorization Code Flow + PKCE.

### Azure AD App Registration

1. **Navigate to Azure Portal** → Azure Active Directory → App registrations
2. **Create new registration:**
   - Name: `HRAgent Frontend` (or your preferred name)
   - Supported account types: Single tenant (your organization only)
   - Redirect URI: 
     - Platform: Single-page application (SPA)
     - URI: `http://localhost:5173` (Vite default dev server)
3. **Configure API permissions:**
   - Add permission → My APIs → Select your backend API
   - Select delegated permission: `access_as_user` (or your custom scope)
4. **Note the following values:**
   - Application (client) ID
   - Directory (tenant) ID

### Environment Configuration

1. **Copy environment template:**
   ```bash
   cp .env.example .env.local
   ```

2. **Configure .env.local with your Azure AD values:**
   ```env
   VITE_AZURE_AD_CLIENT_ID=your-frontend-client-id
   VITE_AZURE_AD_TENANT_ID=your-tenant-id
   VITE_AZURE_AD_API_SCOPE=api://your-backend-client-id/.default
   VITE_API_BASE_URL=http://localhost:5000
   ```

### Authentication Flow

```
┌─────────┐         ┌──────────┐         ┌─────────────┐         ┌─────────┐
│ Browser │         │  MSAL.js │         │  Azure AD   │         │   API   │
└────┬────┘         └─────┬────┘         └──────┬──────┘         └────┬────┘
     │                    │                     │                      │
     │ 1. Click Sign In   │                     │                      │
     ├───────────────────>│                     │                      │
     │                    │ 2. Redirect to      │                      │
     │                    │    Azure AD         │                      │
     │                    ├────────────────────>│                      │
     │                    │                     │ 3. User              │
     │                    │                     │    Authenticates     │
     │                    │                     │    (credentials)     │
     │                    │ 4. Auth code        │                      │
     │                    │<────────────────────┤                      │
     │                    │ 5. Exchange code    │                      │
     │                    │    for tokens       │                      │
     │                    ├────────────────────>│                      │
     │                    │ 6. Access token +   │                      │
     │                    │    ID token         │                      │
     │                    │<────────────────────┤                      │
     │ 7. Authenticated   │                     │                      │
     │<───────────────────┤                     │                      │
     │                    │                     │                      │
     │ 8. API request     │                     │                      │
     │    + Bearer token  │                     │                      │
     ├─────────────────────────────────────────────────────────────────>│
     │                    │                     │                      │
     │                    │                     │ 9. Validate token    │
     │                    │                     │<─────────────────────┤
     │                    │                     │                      │
     │                    │                     │ 10. Protected data   │
     │<─────────────────────────────────────────────────────────────────┤
```

### Token Management

- **Token Storage:** localStorage (automatically managed by MSAL.js)
- **Token Expiration:** Tokens are automatically refreshed when expired (default: 1 hour)
- **Cross-tab Sync:** Authentication state is synchronized across browser tabs
- **Retry Logic:** API client automatically retries with refreshed token on 401 errors

### Development Workflow

1. **Install dependencies:**
   ```bash
   npm install
   ```

2. **Start development server:**
   ```bash
   npm run dev
   ```
   Opens at `http://localhost:5173`

3. **Run tests:**
   ```bash
   npm test
   ```

4. **Build for production:**
   ```bash
   npm run build
   ```

### Troubleshooting

#### Error: "AADSTS50011: The reply URL specified in the request does not match the reply URLs configured"
**Solution:** Add `http://localhost:5173` to your Azure AD app registration's Redirect URIs (SPA platform).

#### Error: "AADSTS65001: The user or administrator has not consented to use the application"
**Solution:** Grant admin consent for API permissions in Azure AD app registration.

#### Error: "Failed to acquire token silently"
**Solution:** This is expected behavior when the user session expires. MSAL will automatically redirect to sign-in.

#### Error: 401 Unauthorized when calling API
**Solution:**
- Verify backend is running (`dotnet run --project HRAgent.Api`)
- Check API scope in .env.local matches backend's expected audience
- Verify backend JWT validation is configured correctly

#### Tokens not persisting across page refresh
**Solution:** Check browser's localStorage is enabled and not cleared by extensions/privacy settings.

### Security Notes

- ⚠️ **Never commit `.env.local`** - it contains sensitive client IDs
- ✅ Token storage in localStorage is secure for SPAs (MSAL uses PKCE flow)
- ✅ Tokens are automatically refreshed before expiration
- ✅ All API requests include Bearer token in Authorization header
- ✅ Backend validates JWT signature, audience, and expiration

### References

- [MSAL.js Documentation](https://github.com/AzureAD/microsoft-authentication-library-for-js)
- [Azure AD SPA Quickstart](https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-spa-overview)
- [Authorization Code Flow + PKCE](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow)

---

## Vite Template Information

See [README.vite-template.md](README.vite-template.md) for original Vite template documentation.
