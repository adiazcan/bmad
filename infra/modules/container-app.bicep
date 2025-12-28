// Container App module
// Creates a container app with configurable settings

@description('Name of the container app')
param containerAppName string

@description('Azure region for the container app')
param location string

@description('Container Apps Environment ID')
param managedEnvironmentId string

@description('Container registry server')
param registryServer string

@description('Container registry username')
param registryUsername string

@description('Container registry password')
@secure()
param registryPassword string

@description('Container image (with tag)')
param containerImage string

@description('Container name')
param containerName string

@description('Target port for ingress')
param targetPort int

@description('CPU cores (e.g., 0.25, 0.5, 1.0)')
param cpuCores string

@description('Memory size (e.g., 0.5Gi, 1Gi, 2Gi)')
param memorySize string

@description('Minimum number of replicas')
@minValue(0)
@maxValue(30)
param minReplicas int = 1

@description('Maximum number of replicas')
@minValue(1)
@maxValue(30)
param maxReplicas int = 10

@description('HTTP concurrency threshold for scaling')
param httpConcurrency int = 100

@description('Environment variables for the container')
param environmentVariables array = []

@description('Secrets for the container app')
@secure()
param secrets array = []

@description('Enable managed identity')
param enableManagedIdentity bool = true

@description('Enable liveness probe')
param enableLivenessProbe bool = true

@description('Enable readiness probe')
param enableReadinessProbe bool = false

@description('Liveness probe path')
param livenessProbePath string = '/health'

@description('Readiness probe path')
param readinessProbePath string = '/health'

@description('Resource tags')
param tags object = {}

resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  tags: tags
  identity: enableManagedIdentity ? {
    type: 'SystemAssigned'
  } : null
  properties: {
    managedEnvironmentId: managedEnvironmentId
    configuration: {
      ingress: {
        external: true
        targetPort: targetPort
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      secrets: secrets
      registries: [
        {
          server: registryServer
          username: registryUsername
          passwordSecretRef: 'acr-password'
        }
      ]
    }
    template: {
      containers: [
        {
          name: containerName
          image: containerImage
          resources: {
            cpu: json(cpuCores)
            memory: memorySize
          }
          env: environmentVariables
          probes: concat(
            enableLivenessProbe ? [
              {
                type: 'liveness'
                httpGet: {
                  path: livenessProbePath
                  port: targetPort
                }
                initialDelaySeconds: 5
                periodSeconds: 10
                failureThreshold: 3
              }
            ] : [],
            enableReadinessProbe ? [
              {
                type: 'readiness'
                httpGet: {
                  path: readinessProbePath
                  port: targetPort
                }
                initialDelaySeconds: 5
                periodSeconds: 5
                failureThreshold: 3
              }
            ] : []
          )
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: string(httpConcurrency)
              }
            }
          }
        ]
      }
    }
  }
}

@description('Container App ID')
output id string = containerApp.id

@description('Container App Name')
output name string = containerApp.name

@description('Container App FQDN')
output fqdn string = containerApp.properties.configuration.ingress.fqdn

@description('Container App URL')
output url string = 'https://${containerApp.properties.configuration.ingress.fqdn}'

@description('Container App System Assigned Identity Principal ID')
output identityPrincipalId string = enableManagedIdentity ? containerApp.identity.principalId : ''
