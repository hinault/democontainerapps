@description('Location des ressources')
param location string = 'canadacentral'

@description('Nom du registre de conteneur')
param acrName string = 'nebularegistry'

@description('Nom de l’environnement de conteneur apps')
param envName string = 'nebula-env'

@description('Nom du workspace Log Analytics')
param logAnalyticsWorkspaceName string = 'nebula-log-analytics'


// Azure Container Registry
resource acr 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }

}

// Log Analytics Workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
   retentionInDays: 30
   features: {
    searchVersion: 1
   }
   sku: {
    name: 'PerGB2018'
   } 
  }
}

// Container Apps Environment
resource env 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: envName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
  
}

// Fonction pour créer une container app
module containerApp 'modules/containerApp.bicep' = [for appName in [
  'nebula-web'
  'nebula-orders-api'
  'nebula-products-api'
]: {
  name: '${appName}-deployment'
  params: {
    name: appName
    location: location
    environmentId: env.id
  }
}]
