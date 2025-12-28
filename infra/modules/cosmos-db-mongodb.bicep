// Azure Cosmos DB for MongoDB API module
// Creates MongoDB-compatible database with autoscale throughput

@description('Name of the Cosmos DB account')
param accountName string

@description('Azure region for the account')
param location string

@description('Database name')
param databaseName string = 'hragent'

@description('Enable automatic failover')
param enableAutomaticFailover bool = true

@description('Enable free tier (only one per subscription, disables autoscale)')
param enableFreeTier bool = false

@description('Use serverless tier instead of provisioned throughput')
param useServerless bool = false

@description('Autoscale max throughput (RU/s) for database - min is 10% of max')
@minValue(1000)
@maxValue(1000000)
param maxAutoscaleThroughput int = 4000

@description('Consistency level')
@allowed([
  'Eventual'
  'ConsistentPrefix'
  'Session'
  'BoundedStaleness'
  'Strong'
])
param consistencyLevel string = 'Session'

@description('Collections to create with their configuration')
param collections array = [
  {
    name: 'conversations'
    shardKey: 'threadId'
    indexes: [
      {
        key: { threadId: 1 }
        name: 'threadId_index'
      }
      {
        key: { userId: 1 }
        name: 'userId_index'
      }
    ]
  }
  {
    name: 'user-patterns'
    shardKey: 'userId'
    indexes: [
      {
        key: { userId: 1 }
        name: 'userId_index'
      }
    ]
  }
]

@description('Resource tags')
param tags object = {}

// Cosmos DB Account with MongoDB API
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: accountName
  location: location
  tags: tags
  kind: 'MongoDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableFreeTier: enableFreeTier
    enableAutomaticFailover: enableAutomaticFailover
    consistencyPolicy: {
      defaultConsistencyLevel: consistencyLevel
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: concat(
      [
        {
          name: 'EnableMongo'
        }
      ],
      useServerless ? [
        {
          name: 'EnableServerless'
        }
      ] : []
    )
    apiProperties: {
      serverVersion: '7.0'
    }
  }
}

// MongoDB Database with autoscale throughput
resource database 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2023-04-15' = {
  parent: cosmosAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
    options: useServerless || enableFreeTier ? {} : {
      autoscaleSettings: {
        maxThroughput: maxAutoscaleThroughput
      }
    }
  }
}

// MongoDB Collections (inherit database-level throughput)
resource mongoCollections 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases/collections@2023-04-15' = [for collection in collections: {
  parent: database
  name: collection.name
  properties: {
    resource: {
      id: collection.name
      shardKey: {
        '${collection.shardKey}': 'Hash'
      }
      indexes: collection.indexes
    }
    // Collections inherit throughput from database-level autoscale
    options: {}
  }
}]

@description('Cosmos DB Account ID')
output id string = cosmosAccount.id

@description('Cosmos DB Account Name')
output name string = cosmosAccount.name

@description('Cosmos DB Endpoint')
output endpoint string = cosmosAccount.properties.documentEndpoint

@description('MongoDB Connection String')
output connectionString string = 'mongodb://${cosmosAccount.name}:${cosmosAccount.listKeys().primaryMasterKey}@${cosmosAccount.name}.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@${cosmosAccount.name}@'

@description('Primary Master Key')
@secure()
output primaryMasterKey string = cosmosAccount.listKeys().primaryMasterKey

@description('Database Name')
output databaseName string = database.name
