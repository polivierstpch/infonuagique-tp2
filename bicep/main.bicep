param location string = resourceGroup().location
param isProd bool = false
param adminDbLogin string
@secure()
param adminDbPassword string
param appName string = 'AutoRapide'

var apiNames = [
  'UsagerAPI'
  'VehiculesAPI'
  'CommandesAPI'
  'FichiersAPI'
  'FavorisAPI'
]

var dbNames = take(apiNames, 3)

module storageAccount 'modules/storageAccount.bicep' = {
  name: 'storage-account'
  params: {
    appName: appName
    location: location
  }
}

var sqlServerName = 'autorapide-${uniqueString(resourceGroup().id)}-sqlserver'

module sqlDatabases 'modules/sqlDatabases.bicep' = {
  name: 'sql-databases'
  params: {
    serverName: sqlServerName
    adminDbLogin: adminDbLogin
    adminDbPassword: adminDbPassword
    dbNames: dbNames
    location: location
  }
}

var envSettings = { 
  name: 'ASPNETCORE_ENVIRONMENT' 
  value: isProd ? 'Production' : 'Development'
}

var apiUrlSettings = [for name in apiNames:{
  name: 'Url${name}'
  value: 'https://${toLower(name)}-${resourceGroup().id}.azurewebsites.net'
}]

var mvcAppSettings = concat([
  envSettings
], apiUrlSettings)

var webAppConfigs = [
  {
    name: 'UsagerAPI'
    scalingType: 'none'
    hasSlot: false
    connectionStrings : [
      {
        connectionString: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname};Database=UsagerAPI;User ID=${adminDbLogin}@${sqlServerName};Password=${adminDbPassword};Trusted_Connection=False;Encrypt=True;'
        name: 'DefaultConnection'
        type: 'SQLServer'
      }
    ]
    appSettings: [
      envSettings
    ]
  }
  {
    name: 'VehiculesAPI'
    scalingType: 'none'
    hasSlot: false
    connectionStrings : [
      {
        connectionString: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname};Database=VehiculesAPI;User ID=${adminDbLogin}@${sqlServerName};Password=${adminDbPassword};Trusted_Connection=False;Encrypt=True;'
        name: 'DefaultConnection'
        type: 'SQLServer'
      }
    ]
    appSettings: [
      envSettings
    ]
  }
  {
    name: 'CommandesAPI'
    scalingType: 'none'
    hasSlot: false
    connectionStrings : [
      {
        connectionString: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname};Database=CommandesAPI;User ID=${adminDbLogin}@${sqlServerName};Password=${adminDbPassword};Trusted_Connection=False;Encrypt=True;'
        name: 'DefaultConnection'
        type: 'SQLServer'
      }
    ]
    appSettings: [
      envSettings
    ]
  }
  {
    name: 'FichiersAPI'
    scalingType: 'manual'
    hasSlot: false
    connectionStrings : []
    appSettings: [
      envSettings
    ]
  }
  {
    name: 'FavorisAPI'
    scalingType: 'none'
    hasSlot: false
    connectionStrings : []
    appSettings: [
      envSettings
    ]
  }
  {
    name: '${appName}MVC'
    scalingType: 'none'
    hasSlot: false
    connectionStrings : []
    appSettings: mvcAppSettings
  }
]

module webApps 'modules/webapp.bicep' = [for config in webAppConfigs: {
  name: '${config.name}-deploy'
  params: {
    appName: config.name
    location: location
    scalingType: config.scalingType
    hasSlot: config.hasSlot
    connectionStrings: config.connectionStrings
    appSettings: config.appSettings
  }
}]
