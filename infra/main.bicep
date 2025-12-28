// Azure Container Apps deployment for HRAgent
// Modular infrastructure-as-code orchestration

targetScope = 'resourceGroup'

// ============================================================================
// PARAMETERS
// ============================================================================

@description('Azure region for resources')
param location string = resourceGroup().location

@description('Environment name (dev, staging, prod)')
@minLength(3)
@maxLength(10)
param environmentName string

// Note: ACR, App Insights, DocumentDB, and Blob Storage are created by modules
// Connection strings and credentials are generated automatically as module outputs

@description('Factorial HR API key')
@secure()
param factorialApiKey string

@description('MongoDB administrator password')
@secure()
@minLength(8)
@maxLength(128)
param mongoAdminPassword string

@description('Azure AD Tenant ID')
param azureAdTenantId string

@description('Azure AD API Client ID')
param azureAdApiClientId string

@description('Azure AD UI Client ID')
param azureAdUiClientId string

@description('Docker image tag for backend')
param backendImageTag string = 'latest'

@description('Docker image tag for frontend')
param frontendImageTag string = 'latest'

@description('Resource tags')
param tags object = {
  environment: environmentName
  project: 'hragent'
  managedBy: 'bicep'
}

// ============================================================================
// VARIABLES
// ============================================================================

// ============================================================================
// MODULE: AZURE CONTAINER REGISTRY
// ============================================================================

module acr 'modules/acr.bicep' = {
  name: 'acr-deployment'
  params: {
    registryName: '${environmentName}hragentreg'
    location: location
    skuName: environmentName == 'prod' ? 'Standard' : 'Basic'
    adminUserEnabled: true
    publicNetworkAccess: 'Enabled'
    tags: tags
  }
}

// ============================================================================
// MODULE: LOG ANALYTICS WORKSPACE
// ============================================================================

module logAnalytics 'modules/log-analytics.bicep' = {
  name: 'log-analytics-deployment'
  params: {
    workspaceName: '${environmentName}-hragent-logs'
    location: location
    skuName: 'PerGB2018'
    retentionInDays: 30
    tags: tags
  }
}

// ============================================================================
// MODULE: APPLICATION INSIGHTS
// ============================================================================

module appInsights 'modules/app-insights.bicep' = {
  name: 'app-insights-deployment'
  params: {
    appInsightsName: '${environmentName}-hragent-insights'
    location: location
    workspaceResourceId: logAnalytics.outputs.id
    applicationType: 'web'
    retentionInDays: 90
    samplingPercentage: environmentName == 'prod' ? 100 : 50
    tags: tags
  }
}

// ============================================================================
// MODULE: AZURE COSMOS DB FOR MONGODB VCORE
// ============================================================================

module documentDb 'modules/document-db-mongodb.bicep' = {
  name: 'document-db-deployment'
  params: {
    clusterName: '${environmentName}-hragent-mongo'
    location: location
    administratorUsername: 'mongoadmin'
    administratorPassword: mongoAdminPassword
    serverVersion: '7.0'
    nodeCount: environmentName == 'prod' ? 'M40' : 'M25' // Prod: M40 (4 vCores, 32GB), Dev: M25 (2 vCores, 8GB)
    shardCount: environmentName == 'prod' ? 2 : 1 // Prod: 2 shards for 200 users, Dev: 1 shard
    enableHa: environmentName == 'prod' // High availability for production only
    storage: environmentName == 'prod' ? 256 : 128 // Prod: 256GB, Dev: 128GB
    tags: tags
  }
}

// ============================================================================
// MODULE: AZURE BLOB STORAGE
// ============================================================================

module blobStorage 'modules/blob-storage.bicep' = {
  name: 'blob-storage-deployment'
  params: {
    storageAccountName: '${environmentName}hragentstore'
    location: location
    skuName: 'Standard_LRS'
    accessTier: 'Cool'
    enableVersioning: true
    blobSoftDeleteRetentionDays: 30
    containerSoftDeleteRetentionDays: 7
    containers: [
      {
        name: 'audit-logs'
        publicAccess: 'None'
        immutabilityPeriodInDays: 2555 // 7 years for compliance
      }
    ]
    tags: tags
  }
}

// ============================================================================
// MODULE: KEY VAULT
// ============================================================================

module keyVault 'modules/key-vault.bicep' = {
  name: 'key-vault-deployment'
  params: {
    keyVaultName: '${environmentName}-hragent-kv'
    location: location
    tenantId: azureAdTenantId
    managedIdentityPrincipalId: '' // Will be updated after API app is created
    skuName: 'standard'
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    enableRbacAuthorization: true
    tags: tags
  }
}

// ============================================================================

// MODULE: CONTAINER APPS ENVIRONMENT
// ============================================================================

module containerAppEnv 'modules/container-apps-environment.bicep' = {
  name: 'container-apps-env-deployment'
  params: {
    environmentName: '${environmentName}-hragent-env'
    location: location
    logAnalyticsCustomerId: logAnalytics.outputs.customerId
    logAnalyticsPrimarySharedKey: logAnalytics.outputs.primarySharedKey
    tags: tags
  }
}

// ============================================================================
// MODULE: BACKEND CONTAINER APP (API)
// ============================================================================

module apiApp 'modules/container-app.bicep' = {
  name: 'api-container-app-deployment'
  params: {
    containerAppName: '${environmentName}-hragent-api'
    location: location
    managedEnvironmentId: containerAppEnv.outputs.id
    registryServer: acr.outputs.loginServer
    registryUsername: acr.outputs.adminUsername
    registryPassword: acr.outputs.adminPassword
    containerImage: '${acr.outputs.loginServer}/hragent-api:${backendImageTag}'
    containerName: 'api'
    targetPort: 8080
    cpuCores: '0.5'
    memorySize: '1Gi'
    minReplicas: 1
    maxReplicas: 10
    httpConcurrency: 100
    enableManagedIdentity: true
    enableLivenessProbe: true
    enableReadinessProbe: true
    livenessProbePath: '/health'
    readinessProbePath: '/health'
    secrets: [
      {
        name: 'documentdb-connection'
        value: cosmosDb.outputs.connectionString
      }
      {
        name: 'blobstorage-connection'
        value: blobStorage.outputs.connectionString
      }
      {
        name: 'appinsights-connection'
        value: appInsights.outputs.connectionString
      }
      {
        name: 'factorial-api-key'
        value: factorialApiKey
      }
      {
        name: 'acr-password'
        value: acr.outputs.adminPassword
      }
    ]
    environmentVariables: [
      {
        name: 'DocumentDb__ConnectionString'
        secretRef: 'documentdb-connection'
      }
      {
        name: 'BlobStorage__ConnectionString'
        secretRef: 'blobstorage-connection'
      }
      {
        name: 'ApplicationInsights__ConnectionString'
        secretRef: 'appinsights-connection'
      }
      {
        name: 'AzureAd__Instance'
        value: 'https://login.microsoftonline.com/'
      }
      {
        name: 'AzureAd__TenantId'
        value: azureAdTenantId
      }
      {
        name: 'AzureAd__ClientId'
        value: azureAdApiClientId
      }
      {
        name: 'AzureAd__Audience'
        value: 'api://${azureAdApiClientId}'
      }
      {
        name: 'Factorial__BaseUrl'
        value: 'https://api.factorialhr.com'
      }
      {
        name: 'Factorial__ApiKey'
        secretRef: 'factorial-api-key'
      }
      {
        name: 'Factorial__ApiVersion'
        value: '2025-10-01'
      }
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'Production'
      }
      {
        name: 'ASPNETCORE_URLS'
        value: 'http://+:8080'
      }
    ]
    tags: tags
  }
}

// ============================================================================
// MODULE: FRONTEND CONTAINER APP (UI)
// ============================================================================

module uiApp 'modules/container-app.bicep' = {
  name: 'ui-container-app-deployment'
  params: {
    containerAppName: '${environmentName}-hragent-ui'
    location: location
    managedEnvironmentId: containerAppEnv.outputs.id
    registryServer: acr.outputs.loginServer
    registryUsername: acr.outputs.adminUsername
    registryPassword: acr.outputs.adminPassword
    containerImage: '${acr.outputs.loginServer}/hragent-ui:${frontendImageTag}'
    containerName: 'ui'
    targetPort: 80
    cpuCores: '0.25'
    memorySize: '0.5Gi'
    minReplicas: 1
    maxReplicas: 3
    httpConcurrency: 200
    enableManagedIdentity: false
    enableLivenessProbe: true
    enableReadinessProbe: false
    livenessProbePath: '/health'
    secrets: [
      {
        name: 'acr-password'
        value: acr.outputs.adminPassword
      }
    ]
    environmentVariables: [
      {
        name: 'REACT_APP_API_URL'
        value: apiApp.outputs.url
      }
      {
        name: 'REACT_APP_MSAL_CLIENT_ID'
        value: azureAdUiClientId
      }
      {
        name: 'REACT_APP_MSAL_AUTHORITY'
        value: 'https://login.microsoftonline.com/${azureAdTenantId}'
      }
      {
        name: 'REACT_APP_MSAL_REDIRECT_URI'
        value: uiApp.outputs.url
      }
    ]
    tags: tags
  }
}

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Backend API URL')
output apiUrl string = apiApp.outputs.url

@description('Frontend UI URL')
output uiUrl string = uiApp.outputs.url

@description('Backend API FQDN')
output apiFqdn string = apiApp.outputs.fqdn

@description('Frontend UI FQDN')
output uiFqdn string = uiApp.outputs.fqdn

@description('Log Analytics Workspace ID')
output logAnalyticsWorkspaceId string = logAnalytics.outputs.id

@description('Container Apps Environment ID')
output containerAppEnvId string = containerAppEnv.outputs.id

@description('Container Apps Environment Default Domain')
output containerAppEnvDefaultDomain string = containerAppEnv.outputs.defaultDomain

@description('Backend API Identity Principal ID')
output apiIdentityPrincipalId string = apiApp.outputs.identityPrincipalId

@description('Key Vault ID')
output keyVaultId string = keyVault.outputs.id

@description('Key Vault Name')
output keyVaultName string = keyVault.outputs.name

@description('Key Vault URI')
output keyVaultUri string = keyVault.outputs.uri

@description('Cosmos DB Cluster Name')
output cosmosDbClusterName string = cosmosDb.outputs.name

@description('Cosmos DB Endpoint')
output cosmosDbEndpoint string = cosmosDb.outputs.endpoint

@description('Blob Storage Account Name')
output blobStorageAccountName string = blobStorage.outputs.name

@description('Blob Storage Primary Endpoint')
output blobStoragePrimaryEndpoint string = blobStorage.outputs.primaryBlobEndpoint

@description('Container Registry Login Server')
output acrLoginServer string = acr.outputs.loginServer

@description('Container Registry Name')
output acrName string = acr.outputs.name

@description('Application Insights Name')
output appInsightsName string = appInsights.outputs.name

@description('Application Insights Instrumentation Key')
@secure()
output appInsightsInstrumentationKey string = appInsights.outputs.instrumentationKey
