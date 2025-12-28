# Azure Container Apps Deployment

This directory contains Infrastructure-as-Code (IaC) templates and deployment scripts for deploying HRAgent to Azure Container Apps.

## Architecture

The deployment creates:
- **Azure Container Apps Environment**: Managed Kubernetes environment
- **Backend Container App**: .NET 10 API (1-10 replicas with auto-scaling)
- **Frontend Container App**: React + Vite UI (1-3 replicas with auto-scaling)
- **Log Analytics Workspace**: Centralized logging
- **Container Registry Integration**: Automatic image pulls from ACR

## Modular Bicep Structure

The infrastructure uses a modular approach for better maintainability and reusability:

```
infra/
├── main.bicep                              # Orchestrator - calls all modules
├── parameters.dev.json                     # Development environment config
├── parameters.prod.json                    # Production environment config
├── deploy.sh                               # Automated deployment script
└── modules/                                # Reusable Bicep modules
    ├── acr.bicep                           # Azure Container Registry
    ├── app-insights.bicep                  # Application Insights with alerts
    ├── cosmos-db-mongodb.bicep             # Azure Cosmos DB (MongoDB API)
    ├── blob-storage.bicep                  # Azure Blob Storage with immutability
    ├── log-analytics.bicep                 # Log Analytics Workspace
    ├── container-apps-environment.bicep    # Container Apps Environment
    └── container-app.bicep                 # Reusable Container App (API/UI)
```

### Module Benefits

1. **Complete Infrastructure**: All resources deployed from scratch (no external dependencies)
2. **Reusability**: Container App module is used for both backend and frontend
3. **Maintainability**: Each module has a single responsibility
4. **Testability**: Modules can be tested independently
5. **Clarity**: Main template is easier to read and understand
6. **Flexibility**: Easy to add new container apps or modify existing ones

### Module Descriptions

- **acr.bicep**: Azure Container Registry
  - Stores Docker images for API and UI
  - Basic SKU for dev, Standard for prod
  - Admin credentials for authentication
  - 30-day image retention policy
  - Webhook for automated deployments

- **app-insights.bicep**: Application Insights with monitoring alerts
  - APM and telemetry collection
  - Linked to Log Analytics Workspace
  - High error rate alerts (>5%)
  - High response time alerts (>3s)
  - 90-day data retention

- **cosmos-db-mongodb.bicep**: Azure Cosmos DB for MongoDB API with autoscale
  - Creates MongoDB-compatible database with autoscale provisioned throughput
  - **Dev environment**: Free tier (1000 RU/s limit) OR 400-4000 RU/s autoscale
  - **Production environment**: 2000-20000 RU/s autoscale for 200 concurrent users
  - Configures collections with shard keys (threadId, userId) and indexes
  - Automatic failover enabled for high availability
  - Outputs connection string for applications
  - MongoDB 7.0 API compatibility

- **blob-storage.bicep**: Azure Blob Storage for audit logs
  - Creates storage account with immutable containers
  - 7-year immutability policy for compliance
  - Blob versioning and soft delete enabled
  - Cool tier for cost optimization

- **log-analytics.bicep**: Log Analytics Workspace
  - Centralized logging for Container Apps
  - 30-day retention by default
  - Outputs workspace credentials

- **container-apps-environment.bicep**: Container Apps Environment
  - Managed Kubernetes environment
  - Integrated with Log Analytics
  - Shared infrastructure for all container apps

- **container-app.bicep**: Reusable Container App (used twice)
  - Parameterized for API or UI deployment
  - Configurable scaling, resources, probes
  - ACR integration with credentials

## Prerequisites

### Required Tools
- Azure CLI (`az`) version 2.50.0 or higher
- Docker (for local builds)
- jq (for JSON parsing in scripts)

### Required Azure Resources (Created Separately)
These resources must exist before deployment:
1. **Azure Container Registry** (ACR) - Store Docker images
2. **Azure DocumentDB** - MongoDB-compatible database (Story 1.5)
3. **Azure Blob Storage** - Audit logs storage (Story 1.6)
4. **Azure Key Vault** - Secrets management
5. **Application Insights** - Application monitoring

### Azure Permissions
- Contributor role on target subscription
- Access to Azure Key Vault for secrets

## Quick Start

⚠️ **IMPORTANT: Validation Required Before Deployment**

Before deploying to any environment, you MUST:
1. **Validate Bicep syntax**: Catch template errors early
2. **Run what-if analysis**: Preview changes to avoid surprises
3. **Review parameter files**: Ensure placeholders are replaced with actual values

### 1. Configure Parameters

Edit `parameters.dev.json` or `parameters.prod.json` and update:
- `azureAdTenantId`: Your Azure AD tenant ID
- `azureAdApiClientId`: API app registration client ID
- `azureAdUiClientId`: UI app registration client ID
- `acrName`: Your Azure Container Registry name
- Key Vault references (subscription ID, resource group, vault name)

### 2. Store Secrets in Key Vault

```bash
# Azure DocumentDB connection string
az keyvault secret set \
  --vault-name your-keyvault \
  --name documentdb-connection-string \
  --value "mongodb://your-connection-string"

# Blob Storage connection string
az keyvault secret set \
  --vault-name your-keyvault \
  --name blobstorage-connection-string \
  --value "DefaultEndpointsProtocol=https;AccountName=..."

# Application Insights connection string
az keyvault secret set \
  --vault-name your-keyvault \
  --name appinsights-connection-string \
  --value "InstrumentationKey=..."

# Factorial HR API key
az keyvault secret set \
  --vault-name your-keyvault \
  --name factorial-api-key \
  --value "your-factorial-api-key"
```

### 3. Build and Push Docker Images

```bash
# Log in to Azure Container Registry
az acr login --name your-acr-name

# Build and push backend image
cd /path/to/project-root
docker build --platform linux/amd64 \
  -t your-acr-name.azurecr.io/hragent-api:latest \
  -f HRAgent.Api/Dockerfile .
docker push your-acr-name.azurecr.io/hragent-api:latest

# Build and push frontend image
docker build --platform linux/amd64 \
  -t your-acr-name.azurecr.io/hragent-ui:latest \
  -f hragent-ui/Dockerfile ./hragent-ui
docker push your-acr-name.azurecr.io/hragent-ui:latest
```

### 4. Deploy Infrastructure

```bash
# Deploy to development environment
./infra/deploy.sh dev westus2

# Deploy to production environment
./infra/deploy.sh prod westus2
```

The script will:
1. Create resource group if it doesn't exist
2. Validate Bicep template
3. Show what-if analysis (preview changes)
4. Prompt for confirmation
5. Deploy infrastructure
6. Verify health endpoints

## Manual Deployment

If you prefer manual steps:

```bash
# 1. Login to Azure
az login

# 2. Create resource group
az group create --name rg-hragent-dev --location westus2

# 3. Validate Bicep template
az deployment group validate \
  --resource-group rg-hragent-dev \
  --template-file infra/main.bicep \
  --parameters infra/parameters.dev.json

# 4. Preview changes (what-if)
az deployment group what-if \
  --resource-group rg-hragent-dev \
  --template-file infra/main.bicep \
  --parameters infra/parameters.dev.json

# 5. Deploy
az deployment group create \
  --resource-group rg-hragent-dev \
  --template-file infra/main.bicep \
  --parameters infra/parameters.dev.json

# 6. Get outputs
az deployment group show \
  --resource-group rg-hragent-dev \
  --name main \
  --query properties.outputs
```

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/docker-build-push.yml`) automates:
1. Building Docker images for backend and frontend
2. Pushing images to Azure Container Registry
3. Deploying to development environment on main branch pushes

### Required GitHub Secrets

Configure these secrets in GitHub repository settings:

- `ACR_LOGIN_SERVER`: Your ACR login server (e.g., `youracr.azurecr.io`)
- `AZURE_CLIENT_ID`: Service principal client ID
- `AZURE_CLIENT_SECRET`: Service principal client secret
- `AZURE_TENANT_ID`: Azure AD tenant ID
- `AZURE_SUBSCRIPTION_ID`: Azure subscription ID

### Create Service Principal

```bash
# Create service principal with AcrPush and Contributor roles
az ad sp create-for-rbac \
  --name github-actions-hragent \
  --role Contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/rg-hragent-dev \
  --sdk-auth

# Grant ACR push permissions
az role assignment create \
  --assignee {client-id} \
  --role AcrPush \
  --scope /subscriptions/{subscription-id}/resourceGroups/{rg}/providers/Microsoft.ContainerRegistry/registries/{acr-name}
```

## Verification

### Check Container App Status

```bash
# Get backend app FQDN
az containerapp show \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --query properties.configuration.ingress.fqdn

# Get frontend app FQDN
az containerapp show \
  --name dev-hragent-ui \
  --resource-group rg-hragent-dev \
  --query properties.configuration.ingress.fqdn

# Check revision history
az containerapp revision list \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --output table
```

### Test Health Endpoints

```bash
# Test backend health
API_URL=$(az containerapp show --name dev-hragent-api -g rg-hragent-dev --query properties.configuration.ingress.fqdn -o tsv)
curl https://$API_URL/health

# Test frontend health
UI_URL=$(az containerapp show --name dev-hragent-ui -g rg-hragent-dev --query properties.configuration.ingress.fqdn -o tsv)
curl https://$UI_URL/health
```

### View Logs

```bash
# Stream backend logs
az containerapp logs show \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --follow

# Stream frontend logs
az containerapp logs show \
  --name dev-hragent-ui \
  --resource-group rg-hragent-dev \
  --follow
```

## Auto-Scaling Configuration

### Backend API Scaling Rules
- **Min Replicas**: 1 (always warm, <200ms response)
- **Max Replicas**: 10 (handles 200 concurrent users)
- **Trigger**: HTTP concurrency >100 requests per replica
- **Scale Down**: After 5 minutes of low traffic

### Frontend UI Scaling Rules
- **Min Replicas**: 1 (always available)
- **Max Replicas**: 3 (static content serves fast)
- **Trigger**: HTTP concurrency >200 requests per replica
- **Scale Down**: After 5 minutes of low traffic

## Updating Container Apps

### Deploy New Image Version

```bash
# Update backend with new image tag
az containerapp update \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --image your-acr-name.azurecr.io/hragent-api:v1.2.0

# Update frontend with new image tag
az containerapp update \
  --name dev-hragent-ui \
  --resource-group rg-hragent-dev \
  --image your-acr-name.azurecr.io/hragent-ui:v1.2.0
```

### Blue-Green Deployment

```bash
# Create new revision without traffic
az containerapp revision copy \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --image your-acr-name.azurecr.io/hragent-api:v1.2.0

# Split traffic for testing (90% old, 10% new)
az containerapp ingress traffic set \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --revision-weight old-revision=90 new-revision=10

# Full cutover to new revision
az containerapp ingress traffic set \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --revision-weight new-revision=100
```

### Rollback

```bash
# List revisions
az containerapp revision list \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --output table

# Activate previous revision
az containerapp revision activate \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --revision {previous-revision-name}
```

## Troubleshooting

### Container Won't Start

```bash
# Check container logs
az containerapp logs show \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --tail 100

# Check revision status
az containerapp revision show \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --revision {revision-name}
```

### Health Checks Failing

```bash
# Exec into running container (if available)
az containerapp exec \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --command /bin/sh

# Check environment variables
az containerapp show \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --query properties.template.containers[0].env
```

### Image Pull Errors

```bash
# Verify ACR credentials
az acr credential show --name your-acr-name

# Update container app registry credentials
az containerapp registry set \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --server your-acr-name.azurecr.io \
  --username {username} \
  --password {password}
```

## Cost Optimization

### Development Environment
- Scale backend to zero outside business hours: 8am-6pm weekdays
- Scale frontend min replicas to 0 for dev/test (cold start acceptable)
- Use Basic tier ACR instead of Standard

```bash
# Set scale-to-zero for dev environment
az containerapp update \
  --name dev-hragent-api \
  --resource-group rg-hragent-dev \
  --min-replicas 0

# Schedule scaling (requires Azure Logic App or Function)
# Scale up at 8am: Set min replicas to 1
# Scale down at 6pm: Set min replicas to 0
```

### Production Environment
- Keep min replicas = 1 for <200ms cold start avoidance
- Monitor Application Insights for right-sizing (CPU/memory usage)
- Use consumption plan (pay for vCPU-seconds used)

## Security Best Practices

1. **Secrets Management**
   - Store all secrets in Azure Key Vault
   - Use managed identity for Key Vault access
   - Never commit secrets to git

2. **Network Security**
   - Enable Azure Container Apps environment with VNET (optional)
   - Configure custom domains with managed certificates
   - Use Azure Front Door for DDoS protection (future)

3. **Container Security**
   - Run containers as non-root user
   - Use minimal base images (alpine)
   - Scan images for vulnerabilities with ACR scanning
   - Regularly update base images

4. **Access Control**
   - Use Azure AD authentication for API
   - Enable managed identity for service-to-service auth
   - Follow principle of least privilege

## Monitoring & Observability

### Application Insights

```bash
# Query for errors
az monitor app-insights query \
  --app {app-insights-name} \
  --resource-group rg-hragent-dev \
  --analytics-query "exceptions | where timestamp > ago(1h) | summarize count() by problemId"

# Query for performance
az monitor app-insights query \
  --app {app-insights-name} \
  --resource-group rg-hragent-dev \
  --analytics-query "requests | where timestamp > ago(1h) | summarize avg(duration), percentile(duration, 95) by name"
```

### Alerts

Configure alerts for:
- Error rate >1%
- Request latency p95 >3 seconds
- Container restart loops
- Auto-scaling hitting max replicas (capacity planning)

## Additional Resources

- [Azure Container Apps Documentation](https://learn.microsoft.com/en-us/azure/container-apps/)
- [Bicep Language Reference](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [Docker Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [.NET 10 Deployment Guide](https://learn.microsoft.com/en-us/dotnet/core/deploying/)
