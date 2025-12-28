// Azure Blob Storage module
// Creates storage account for immutable audit logs

@description('Name of the storage account')
@minLength(3)
@maxLength(24)
param storageAccountName string

@description('Azure region for the storage account')
param location string

@description('Storage account SKU')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
])
param skuName string = 'Standard_LRS'

@description('Storage account access tier')
@allowed([
  'Hot'
  'Cool'
])
param accessTier string = 'Cool'

@description('Enable blob versioning')
param enableVersioning bool = true

@description('Soft delete retention days for blobs')
@minValue(1)
@maxValue(365)
param blobSoftDeleteRetentionDays int = 30

@description('Soft delete retention days for containers')
@minValue(1)
@maxValue(365)
param containerSoftDeleteRetentionDays int = 7

@description('Containers to create')
param containers array = [
  {
    name: 'audit-logs'
    publicAccess: 'None'
    immutabilityPeriodInDays: 2555 // 7 years for compliance
  }
]

@description('Resource tags')
param tags object = {}

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  kind: 'StorageV2'
  properties: {
    accessTier: accessTier
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

// Blob Service with versioning and soft delete
resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    isVersioningEnabled: enableVersioning
    deleteRetentionPolicy: {
      enabled: true
      days: blobSoftDeleteRetentionDays
    }
    containerDeleteRetentionPolicy: {
      enabled: true
      days: containerSoftDeleteRetentionDays
    }
  }
}

// Blob Containers
resource blobContainers 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = [for container in containers: {
  parent: blobService
  name: container.name
  properties: {
    publicAccess: container.publicAccess
    metadata: {}
  }
}]

// Immutability Policy for audit logs container
resource immutabilityPolicy 'Microsoft.Storage/storageAccounts/blobServices/containers/immutabilityPolicies@2023-01-01' = [for (container, i) in containers: if (contains(container, 'immutabilityPeriodInDays')) {
  parent: blobContainers[i]
  name: 'default'
  properties: {
    immutabilityPeriodSinceCreationInDays: container.immutabilityPeriodInDays
    allowProtectedAppendWrites: true
    allowProtectedAppendWritesAll: false
  }
}]

@description('Storage Account ID')
output id string = storageAccount.id

@description('Storage Account Name')
output name string = storageAccount.name

@description('Primary Blob Endpoint')
output primaryBlobEndpoint string = storageAccount.properties.primaryEndpoints.blob

@description('Storage Account Connection String')
@secure()
output connectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'

@description('Primary Access Key')
@secure()
output primaryAccessKey string = storageAccount.listKeys().keys[0].value
