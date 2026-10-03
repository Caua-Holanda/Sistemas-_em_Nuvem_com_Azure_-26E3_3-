# Provisionamento Azure

O arquivo `main.bicep` cria os componentes do MVP em um único grupo de recursos: Storage com site estático e versionamento, VM Linux B1s, Azure SQL serverless com limite gratuito, Key Vault, VNet/NSG, Log Analytics, Azure Monitor Agent e Application Insights conectado ao workspace.

## Implantar

```powershell
az login
az group create --name rg-techstore-dev --location eastus
az deployment group create --resource-group rg-techstore-dev --template-file infra/main.bicep --parameters prefix=techstore-seunome vmSshPublicKey="SUA_CHAVE_PUBLICA" sqlAdminPassword="SENHA_FORTE" sshSourceCidr="SEU_IP/32"
```

Não coloque senha no arquivo de parâmetros. A conexão do banco é salva no Key Vault. A VM usa identidade gerenciada para buscar o segredo durante a implantação da aplicação.

Depois do provisionamento, copie as saídas `apiUrl`, `apiHost`, `frontendUrl`, `storageAccountName`, `vmName` e `keyVaultName` para as variáveis do ambiente `production` do GitHub. O workflow da raiz compila `FRONT/`, publica o build no contêiner `$web` e usa o Caddy na própria VM para fornecer HTTPS gratuito à API. Pare/desaloque a VM ao terminar os testes para não consumir créditos.
