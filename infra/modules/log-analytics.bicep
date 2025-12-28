// Log Analytics Workspace module
// Creates a workspace for Container Apps logging and monitoring

@description('Name of the Log Analytics workspace')
param workspaceName string

@description('Azure region for the workspace')
param location string

@description('SKU name for the workspace')
@allowed([
  'PerGB2018'
  'Free'
  'Standalone'
  'PerNode'
  'Premium'
])
param skuName string = 'PerGB2018'

@description('Data retention in days')
@minValue(30)
@maxValue(730)
param retentionInDays int = 30

@description('Resource tags')
param tags object = {}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: workspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: skuName
    }
    retentionInDays: retentionInDays
  }
}

@description('Log Analytics Workspace ID')
output id string = logAnalytics.id

@description('Log Analytics Workspace Customer ID')
output customerId string = logAnalytics.properties.customerId

@description('Log Analytics Workspace Primary Shared Key')
output primarySharedKey string = logAnalytics.listKeys().primarySharedKey

@description('Log Analytics Workspace Name')
output name string = logAnalytics.name
