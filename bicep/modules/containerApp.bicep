param name string
param location string
param environmentId string

resource app 'Microsoft.App/containerApps@2022-03-01' = {
  name: name
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      ingress: {
        external: true
        targetPort: 80
      }
    }
    template: {
      containers: [
        {
          name: name
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        }
      ]
    }
  }
}
