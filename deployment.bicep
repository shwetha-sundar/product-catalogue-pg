param location string = resourceGroup().location
param tenant string = subscription().tenantId
param functionAppName string = 'productapp${uniqueString(resourceGroup().id)}'
param postgresServerName string = 'postgres${uniqueString(resourceGroup().id)}'
param postgresDbName string = 'productdb'
param userAssignedIdentityName string = 'productapp-identity'

resource uami 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: userAssignedIdentityName
  location: location
}

resource plan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${functionAppName}-plan'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uami.id}': {}
    }
  }
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      appSettings: [
        { name: 'FUNCTIONS_WORKER_RUNTIME', value: 'dotnet-isolated' }
        {
          name: 'ConnectionString'
          value: 'Host=${postgresServerName}.postgres.database.azure.com;Database=${postgresDbName};Ssl Mode=Require;User=${uami.name};Trust Server Certificate=true'
        }
      ]
    }
  }
}

resource postgres 'Microsoft.DBforPostgreSQL/flexibleServers@2022-12-01' = {
  name: postgresServerName
  location: location
  sku: {
    name: 'Standard_D2s_v3'
    tier: 'GeneralPurpose'
    capacity: 2
  }
  properties: {
    version: '14'
    storage: {
      storageSizeGB: 32
    }
    authConfig: {
      activeDirectoryAuth: 'Enabled'
      passwordAuth: 'Disabled'
    }
  }
}

resource entraIdAdmin 'Microsoft.DBforPostgreSQL/flexibleServers/administrators@2022-12-01' = {
  parent: postgres
  name: '548dcbb8-b797-4eee-928d-711e4062b54b'
  properties: {
    principalType: 'ServicePrincipal'
    principalName: uami.name
    tenantId: tenant
  }
}

resource entraIdAdminUser 'Microsoft.DBforPostgreSQL/flexibleServers/administrators@2022-12-01' = {
  parent: postgres
  name: '39bfcaa7-8183-4177-ab49-f1044a95c999'
  properties: {
    principalType: 'User'
    principalName: 'shwethas@microsoft.com'
    tenantId: tenant
  }
}

resource postgresDb 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2022-12-01' = {
  parent: postgres
  name: '${postgresDbName}'
  properties: {}
}

output functionAppIdentity object = functionApp.identity
