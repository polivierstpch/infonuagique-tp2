param location string
param appName string
@allowed([
  'none'
  'manual'
  'auto'
])
param scalingType string = 'none'
param hasSlot bool = false
param appSettings array = []
param connectionStrings array = []

resource servicePlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: '${appName}-plan'
  location: location
  sku: {
    name: scalingType == 'manual' ? 'B1' : scalingType == 'auto' ? 'S1' : 'F1'
  }
}

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: '${appName}-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    serverFarmId: servicePlan.id
    siteConfig: {
      appSettings: appSettings
      connectionStrings: connectionStrings
    }
  }
}

resource stagingWebAppSlot 'Microsoft.Web/sites/slots@2021-03-01' = if (hasSlot) {
  name: '${webApp.name}/staging'
  location: location
  properties: {
    serverFarmId: servicePlan.id
  }
}

var autoScaleSettingsName = '${appName}-autoscalesettings'

var scalingRules = {
  minInstances: 1
  maxInstances: 6
  increaseCpuThreshold: 80
  decreaseCpuThreshold: 50
}

resource autoScaleSettings 'Microsoft.Insights/autoscalesettings@2021-05-01-preview' = if (scalingType == 'auto') {
  name: autoScaleSettingsName
  location: location
  tags: {
    ResourceType: 'AppInsights-AutoScaleSettings'
    Use: 'FrontendClient'
  }
  properties: {
    enabled: true
    name: autoScaleSettingsName
    targetResourceUri: servicePlan.id
    profiles: [
      {
        name: '${appName}-scalingrule'
        capacity: {
          default: string(scalingRules.minInstances)
          minimum: string(scalingRules.minInstances)
          maximum: string(scalingRules.maxInstances)
        }
        rules: [
          {
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricNamespace: 'Microsoft.Web/serverfarms'
              metricResourceUri: servicePlan.id
              operator: 'GreaterThan'
              statistic: 'Average'
              threshold: scalingRules.increaseCpuThreshold
              timeAggregation: 'Average'
              timeGrain: 'PT1M'
              timeWindow: 'PT10M'
            }
          }
          {
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricNamespace: 'Microsoft.Web/serverfarms'
              metricResourceUri: servicePlan.id
              operator: 'GreaterThan'
              statistic: 'Average'
              threshold: scalingRules.decreaseCpuThreshold
              timeAggregation: 'Average'
              timeGrain: 'PT1M'
              timeWindow: 'PT10M'
            }
          }  
        ]
      }
    ]
  }
}

output webAppUrl string = webApp.properties.defaultHostName
