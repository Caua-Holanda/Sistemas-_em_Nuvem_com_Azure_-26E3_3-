targetScope = 'resourceGroup'

@description('Região Azure. Brazil South pode não oferecer todos os SKUs gratuitos; East US é o padrão econômico.')
param location string = 'eastus'

@minLength(3)
@maxLength(12)
param prefix string = 'techstore'

@description('Usuário administrador da VM.')
param vmAdminUsername string = 'azureuser'

@secure()
@description('Chave pública SSH da VM, começando com ssh-ed25519 ou ssh-rsa.')
param vmSshPublicKey string

@description('Usuário administrador lógico do Azure SQL.')
param sqlAdminUsername string = 'techstoreadmin'

@secure()
param sqlAdminPassword string

@description('Restrinja SSH ao seu IP no formato CIDR, por exemplo 200.100.50.25/32.')
param sshSourceCidr string

var suffix = uniqueString(resourceGroup().id)
var storageName = toLower(take('${prefix}${suffix}', 24))
var sqlServerName = toLower('${prefix}-sql-${suffix}')
var vaultName = toLower(take('${prefix}-kv-${suffix}', 24))
var vmName = '${prefix}-api-vm'
var connectionString = 'Server=tcp:${sqlServerName}.database.windows.net,1433;Initial Catalog=TechStoreDb;Persist Security Info=False;User ID=${sqlAdminUsername};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: true
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storage
  name: 'default'
  properties: {
    deleteRetentionPolicy: { enabled: true, days: 7 }
    isVersioningEnabled: true
    staticWebsite: {
      enabled: true
      indexDocument: 'index.html'
      error404Document: 'index.html'
    }
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: 'TechStoreDb'
  location: location
  sku: { name: 'GP_S_Gen5_2', tier: 'GeneralPurpose', family: 'Gen5', capacity: 2 }
  properties: {
    autoPauseDelay: 60
    minCapacity: 0.5
    maxSizeBytes: 34359738368
    useFreeLimit: true
    freeLimitExhaustionBehavior: 'AutoPause'
  }
}

resource allowAzure 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: { startIpAddress: '0.0.0.0', endIpAddress: '0.0.0.0' }
}

resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: vaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: { family: 'A', name: 'standard' }
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enablePurgeProtection: true
    enabledForTemplateDeployment: true
    publicNetworkAccess: 'Enabled'
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: vm.properties.identity.principalId
        permissions: { secrets: [ 'get', 'list' ] }
      }
    ]
  }
}

resource connectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: vault
  name: 'ConnectionStrings--DefaultConnection'
  properties: { value: connectionString }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${prefix}-logs-${suffix}'
  location: location
  properties: { retentionInDays: 30, features: { enableLogAccessUsingOnlyResourcePermissions: true } }
  sku: { name: 'PerGB2018' }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${prefix}-insights-${suffix}'
  location: location
  kind: 'web'
  properties: { Application_Type: 'web', WorkspaceResourceId: workspace.id }
}

resource vnet 'Microsoft.Network/virtualNetworks@2023-09-01' = {
  name: '${prefix}-vnet'
  location: location
  properties: {
    addressSpace: { addressPrefixes: [ '10.10.0.0/16' ] }
    subnets: [ { name: 'api', properties: { addressPrefix: '10.10.1.0/24', networkSecurityGroup: { id: nsg.id } } } ]
  }
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2023-09-01' = {
  name: '${prefix}-api-nsg'
  location: location
  properties: {
    securityRules: [
      { name: 'AllowHttp', properties: { priority: 100, access: 'Allow', direction: 'Inbound', protocol: 'Tcp', sourcePortRange: '*', destinationPortRange: '80', sourceAddressPrefix: 'Internet', destinationAddressPrefix: '*' } }
      { name: 'AllowHttps', properties: { priority: 110, access: 'Allow', direction: 'Inbound', protocol: 'Tcp', sourcePortRange: '*', destinationPortRange: '443', sourceAddressPrefix: 'Internet', destinationAddressPrefix: '*' } }
      { name: 'AllowSshFromAdmin', properties: { priority: 120, access: 'Allow', direction: 'Inbound', protocol: 'Tcp', sourcePortRange: '*', destinationPortRange: '22', sourceAddressPrefix: sshSourceCidr, destinationAddressPrefix: '*' } }
    ]
  }
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2023-09-01' = {
  name: '${prefix}-api-ip'
  location: location
  sku: { name: 'Standard' }
  properties: { publicIPAllocationMethod: 'Static', publicIPAddressVersion: 'IPv4', dnsSettings: { domainNameLabel: toLower('${prefix}-${suffix}') } }
}

resource nic 'Microsoft.Network/networkInterfaces@2023-09-01' = {
  name: '${prefix}-api-nic'
  location: location
  properties: {
    ipConfigurations: [ { name: 'ipconfig', properties: { privateIPAllocationMethod: 'Dynamic', publicIPAddress: { id: publicIp.id }, subnet: { id: vnet.properties.subnets[0].id } } } ]
  }
}

var cloudInit = '''#cloud-config
package_update: true
packages:
  - docker.io
  - jq
runcmd:
  - systemctl enable --now docker
  - usermod -aG docker ${vmAdminUsername}
'''

resource vm 'Microsoft.Compute/virtualMachines@2023-09-01' = {
  name: vmName
  location: location
  identity: { type: 'SystemAssigned' }
  properties: {
    hardwareProfile: { vmSize: 'Standard_B1s' }
    osProfile: {
      computerName: vmName
      adminUsername: vmAdminUsername
      customData: base64(cloudInit)
      linuxConfiguration: {
        disablePasswordAuthentication: true
        ssh: { publicKeys: [ { path: '/home/${vmAdminUsername}/.ssh/authorized_keys', keyData: vmSshPublicKey } ] }
      }
    }
    storageProfile: {
      imageReference: { publisher: 'Canonical', offer: '0001-com-ubuntu-server-jammy', sku: '22_04-lts-gen2', version: 'latest' }
      osDisk: { createOption: 'FromImage', managedDisk: { storageAccountType: 'Standard_LRS' } }
    }
    networkProfile: { networkInterfaces: [ { id: nic.id } ] }
  }
}

resource monitorAgent 'Microsoft.Compute/virtualMachines/extensions@2023-09-01' = {
  parent: vm
  name: 'AzureMonitorLinuxAgent'
  location: location
  properties: { publisher: 'Microsoft.Azure.Monitor', type: 'AzureMonitorLinuxAgent', typeHandlerVersion: '1.0', autoUpgradeMinorVersion: true, enableAutomaticUpgrade: true }
}

resource dcr 'Microsoft.Insights/dataCollectionRules@2023-03-11' = {
  name: '${prefix}-linux-dcr'
  location: location
  properties: {
    dataSources: { syslog: [ { name: 'syslog', facilityNames: [ 'user', 'daemon', 'local0' ], logLevels: [ 'Debug', 'Info', 'Notice', 'Warning', 'Error', 'Critical', 'Alert', 'Emergency' ], streams: [ 'Microsoft-Syslog' ] } ] }
    destinations: { logAnalytics: [ { name: 'workspace', workspaceResourceId: workspace.id } ] }
    dataFlows: [ { streams: [ 'Microsoft-Syslog' ], destinations: [ 'workspace' ] } ]
  }
}

resource dcrAssociation 'Microsoft.Insights/dataCollectionRuleAssociations@2023-03-11' = {
  name: '${vm.name}/Microsoft.Insights/${prefix}-dcr-association'
  properties: { dataCollectionRuleId: dcr.id }
}

output frontendUrl string = blobService.properties.primaryEndpoints.web
output storageAccountName string = storage.name
output apiHost string = publicIp.properties.dnsSettings.fqdn
output apiUrl string = 'https://${publicIp.properties.dnsSettings.fqdn}/api/v1'
output vmName string = vm.name
output keyVaultName string = vault.name
output applicationInsightsName string = appInsights.name
