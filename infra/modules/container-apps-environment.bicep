// Container Apps Environment module
// Creates a managed environment for hosting container apps

@description('Name of the Container Apps environment')
param environmentName string

@description('Azure region for the environment')
param location string

@description('Log Analytics Workspace Customer ID')
param logAnalyticsCustomerId string

@description('Log Analytics Workspace Primary Shared Key')
@secure()
param logAnalyticsPrimarySharedKey string

@description('Resource tags')
param tags object = {}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: environmentName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsCustomerId
        sharedKey: logAnalyticsPrimarySharedKey
      }
    }
  }
}

@description('Container Apps Environment ID')
output id string = containerAppEnv.id

@description('Container Apps Environment Name')
output name string = containerAppEnv.name

@description('Container Apps Environment Default Domain')
output defaultDomain string = containerAppEnv.properties.defaultDomain

@description('Container Apps Environment Static IP')
output staticIp string = containerAppEnv.properties.staticIp
