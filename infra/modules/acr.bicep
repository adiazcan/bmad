// Azure Container Registry module
// Creates ACR for storing Docker images

@description('Name of the container registry')
@minLength(5)
@maxLength(50)
param registryName string

@description('Azure region for the registry')
param location string

@description('Registry SKU')
@allowed([
  'Basic'
  'Standard'
  'Premium'
])
param skuName string = 'Basic'

@description('Enable admin user')
param adminUserEnabled bool = true

@description('Enable zone redundancy (Premium SKU only)')
param zoneRedundancy bool = false

@description('Public network access')
@allowed([
  'Enabled'
  'Disabled'
])
param publicNetworkAccess string = 'Enabled'

@description('Enable anonymous pull')
param anonymousPullEnabled bool = false

@description('Data endpoint enabled (Premium SKU only)')
param dataEndpointEnabled bool = false

@description('Resource tags')
param tags object = {}

// Container Registry
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: registryName
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  properties: {
    adminUserEnabled: adminUserEnabled
    publicNetworkAccess: publicNetworkAccess
    anonymousPullEnabled: anonymousPullEnabled
    zoneRedundancy: (skuName == 'Premium' && zoneRedundancy) ? 'Enabled' : 'Disabled'
    dataEndpointEnabled: (skuName == 'Premium' && dataEndpointEnabled) ? true : false
    networkRuleBypassOptions: 'AzureServices'
    policies: {
      retentionPolicy: {
        status: 'enabled'
        days: 30
      }
      quarantinePolicy: {
        status: 'disabled'
      }
      trustPolicy: {
        status: 'disabled'
      }
    }
  }
}

// Webhook for automated deployments (optional)
resource webhook 'Microsoft.ContainerRegistry/registries/webhooks@2023-01-01-preview' = {
  parent: containerRegistry
  name: 'deploymentWebhook'
  location: location
  properties: {
    status: 'enabled'
    scope: 'hragent-api:latest,hragent-ui:latest'
    actions: [
      'push'
    ]
    serviceUri: 'https://placeholder.com/webhook'
  }
}

@description('Container Registry ID')
output id string = containerRegistry.id

@description('Container Registry Name')
output name string = containerRegistry.name

@description('Login Server')
output loginServer string = containerRegistry.properties.loginServer

@description('Admin Username')
output adminUsername string = containerRegistry.listCredentials().username

@description('Admin Password')
@secure()
output adminPassword string = containerRegistry.listCredentials().passwords[0].value

@description('Admin Password 2')
@secure()
output adminPassword2 string = containerRegistry.listCredentials().passwords[1].value
