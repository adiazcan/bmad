// Azure Application Insights module
// Creates Application Insights for monitoring and telemetry

@description('Name of the Application Insights instance')
param appInsightsName string

@description('Azure region for Application Insights')
param location string

@description('Log Analytics Workspace ID to link to')
param workspaceResourceId string

@description('Application type')
@allowed([
  'web'
  'other'
])
param applicationType string = 'web'

@description('Retention in days')
@minValue(30)
@maxValue(730)
param retentionInDays int = 90

@description('Daily data cap in GB (0 = no cap)')
@minValue(0)
param dailyDataCapInGB int = 0

@description('Disable IP masking (for debugging)')
param disableIpMasking bool = false

@description('Sampling percentage (0-100)')
@minValue(0)
@maxValue(100)
param samplingPercentage int = 100

@description('Resource tags')
param tags object = {}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: applicationType
    WorkspaceResourceId: workspaceResourceId
    RetentionInDays: retentionInDays
    SamplingPercentage: samplingPercentage
    DisableIpMasking: disableIpMasking
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    Request_Source: 'rest'
  }
}

// Alert Rule: High Error Rate
resource errorRateAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${appInsightsName}-high-error-rate'
  location: 'global'
  tags: tags
  properties: {
    description: 'Alert when error rate exceeds 5% of requests'
    severity: 2
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'FailedRequestsPercentage'
          metricName: 'requests/failed'
          operator: 'GreaterThan'
          threshold: 5
          timeAggregation: 'Average'
        }
      ]
    }
  }
}

// Alert Rule: High Response Time
resource responseTimeAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${appInsightsName}-high-response-time'
  location: 'global'
  tags: tags
  properties: {
    description: 'Alert when average response time exceeds 3 seconds'
    severity: 3
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'ResponseTime'
          metricName: 'requests/duration'
          operator: 'GreaterThan'
          threshold: 3000
          timeAggregation: 'Average'
        }
      ]
    }
  }
}

@description('Application Insights ID')
output id string = appInsights.id

@description('Application Insights Name')
output name string = appInsights.name

@description('Instrumentation Key')
@secure()
output instrumentationKey string = appInsights.properties.InstrumentationKey

@description('Connection String')
@secure()
output connectionString string = appInsights.properties.ConnectionString

@description('Application ID')
output applicationId string = appInsights.properties.AppId
