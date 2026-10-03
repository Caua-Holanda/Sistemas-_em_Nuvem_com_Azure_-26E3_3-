# TechStore Cloud

MVP de cadastro de produtos para a Parte 2 do projeto prático de Sistemas em Nuvem com Azure.

## Estrutura

- `API/`: API REST .NET, domínio, persistência, testes, Docker, Bicep e documentação Azure.
- `FRONT/`: frontend Angular com cadastro, consulta, edição e exclusão de produtos na mesma tela.
- `.github/workflows/`: integração contínua e implantação da API na VM e do frontend no Blob Static Website.

## Executar localmente

Em um terminal:

```powershell
cd API
dotnet restore "TechStore Cloud.sln"
dotnet run --project "TechStore Cloud/TechStore Cloud.csproj" --launch-profile http
```

Em outro terminal:

```powershell
cd FRONT
npm install
npm start
```

Acesse `http://localhost:4200`. A API estará em `http://localhost:5093` e o Swagger em `http://localhost:5093/swagger`.

## Documentação da entrega

- [Arquitetura e decisões](API/docs/ARQUITETURA.md)
- [Mapa da rubrica](API/docs/MAPA_RUBRICA.md)
- [Roteiro de evidências](API/docs/ROTEIRO_EVIDENCIAS.md)
- [Infraestrutura como código](API/infra/README.md)
- [Frontend Angular](FRONT/README.md)

## Variáveis para o deploy no GitHub

Crie o ambiente `production` e cadastre os segredos `AZURE_CLIENT_ID`, `AZURE_TENANT_ID` e `AZURE_SUBSCRIPTION_ID`. Cadastre também estas variáveis, usando as saídas do Bicep e os nomes dos recursos:

| Variável | Conteúdo |
|---|---|
| `API_URL` | Saída `apiUrl`, incluindo `/api/v1` |
| `API_HOST` | Saída `apiHost`, sem `https://` |
| `FRONTEND_URL` | Saída `frontendUrl`, sem barra final |
| `AZURE_RESOURCE_GROUP` | Nome do grupo de recursos |
| `AZURE_VM_NAME` | Saída `vmName` |
| `AZURE_KEY_VAULT_NAME` | Saída `keyVaultName` |
| `AZURE_STORAGE_ACCOUNT` | Nome da Storage Account criada pelo Bicep |

O deploy é manual pela aba Actions ou automático ao publicar uma tag iniciada por `v`.
