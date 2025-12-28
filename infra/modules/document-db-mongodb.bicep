// Azure Cosmos DB for MongoDB vCore cluster
// Creates MongoDB-compatible database with dedicated vCore architecture

@description('Name of the MongoDB cluster')
param clusterName string

@description('Azure region for the cluster')
param location string

@description('Administrator username')
param administratorUsername string = 'mongoadmin'

@description('Administrator password')
@secure()
@minLength(8)
@maxLength(128)
param administratorPassword string

@description('MongoDB version')
@allowed(['5.0', '6.0', '7.0'])
param serverVersion string = '7.0'

@description('Compute tier')
@allowed(['M25', 'M30', 'M40', 'M50', 'M60', 'M80', 'M200'])
param nodeCount string = 'M40'

@description('Number of shards')
@minValue(1)
@maxValue(12)
param shardCount int = 1

@description('Enable high availability')
param enableHa bool = true

@description('Storage size in GB')
@minValue(32)
@maxValue(16384)
param storage int = 128

@description('Resource tags')
param tags object = {}

// MongoDB vCore Cluster
resource mongoCluster 'Microsoft.DocumentDB/mongoClusters@2024-07-01' = {
  name: clusterName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorUsername
    administratorLoginPassword: administratorPassword
    serverVersion: serverVersion
    nodeGroupSpecs: [
      {
        kind: 'Shard'
        sku: nodeCount
        diskSizeGB: storage
        enableHa: enableHa
        nodeCount: shardCount
      }
    ]
    publicNetworkAccess: 'Enabled'
  }
}

// Create default database (firewall rules and collections managed via connection string)
resource mongoDatabase 'Microsoft.DocumentDB/mongoClusters/firewallRules@2024-07-01' = {
  parent: mongoCluster
  name: 'AllowAllAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

@description('Cluster ID')
output id string = mongoCluster.id

@description('Cluster Name')
output name string = mongoCluster.name

@description('MongoDB Connection String')
@secure()
output connectionString string = 'mongodb://${administratorUsername}:${administratorPassword}@${mongoCluster.properties.connectionString}'

@description('Cluster Endpoint')
output endpoint string = mongoCluster.properties.connectionString

@description('Earliest Restore Time')
output earliestRestoreTime string = mongoCluster.properties.earliestRestoreTime

