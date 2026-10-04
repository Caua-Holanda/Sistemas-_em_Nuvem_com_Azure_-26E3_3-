# TechStore Cloud

MVP de cadastro de produtos para a Parte 2 do projeto prático de Sistemas em Nuvem com Azure.

## Estrutura

- `API/`: API REST .NET, domínio, persistência, testes, Docker, Bicep e documentação Azure.
- `FRONT/`: frontend Angular com cadastro, consulta, edição e exclusão de produtos na mesma tela.
- `.github/workflows/`: integração contínua e implantação automática da API no App Service e do frontend no Blob Static Website.

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

## Deploy automatizado no GitHub

O workflow `CD - API e Frontend no Azure` é executado após cada push na `main` que altere a API, o frontend ou o próprio workflow. Ele:

1. compila e testa a solução .NET;
2. publica a API no App Service;
3. obtém do Azure o domínio real do App Service;
4. compila o Angular com esse domínio;
5. publica o resultado no contêiner `$web` da Storage Account.

As credenciais OIDC do App Service já são mantidas nos segredos criados pelo Centro de Implantação. Em **Settings > Secrets and variables > Actions**, adicione apenas o segredo de repositório `AZURE_STORAGE_CONNECTION_STRING`, copiando a cadeia de conexão em **Storage Account > Chaves de acesso > Mostrar chaves**. Nunca coloque esse valor no código.

Também é possível executar o workflow manualmente pela aba **Actions**.
