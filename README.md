# TechStore Cloud

MVP de cadastro de produtos desenvolvido para a disciplina Sistemas em Nuvem com Azure.

## Componentes

- Frontend Angular no Azure Blob Storage Static Website.
- API REST .NET 8 no Azure App Service.
- Banco de dados Azure SQL Database.
- Segredos no Azure Key Vault.
- Monitoramento com Application Insights e Log Analytics.
- CI/CD com GitHub Actions.

## Links

- Frontend: https://sttechstorecaua2026.z47.web.core.windows.net
- API: https://app-techstore-api-caua-bmgyf2duafbhfzcs.chilecentral-01.azurewebsites.net/swagger/index.html


## Executar localmente

API:

```powershell
cd API
dotnet restore "TechStore Cloud.sln"
dotnet run --project "TechStore Cloud/TechStore Cloud.csproj" --launch-profile http
```

Frontend:

```powershell
cd FRONT
npm install
npm start
```

O frontend local abre em `http://localhost:4200` e a API em `http://localhost:5093`.

## Deploy

Um push na branch `main` executa os workflows de CI e CD. O pipeline testa a API, compila o Angular e publica os dois componentes no Azure.
