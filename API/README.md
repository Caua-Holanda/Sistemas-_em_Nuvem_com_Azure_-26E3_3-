# TechStore Cloud

MVP acadêmico para cadastro de produtos na Microsoft Azure. Inclui API REST em .NET 8, frontend Angular estático, Azure SQL, VM Linux, observabilidade, segurança, IaC e CI/CD.

## CRUD disponível

| Método | Rota | Função |
|---|---|---|
| GET | `/api/v1/products` | Lista paginada com busca por nome/SKU |
| GET | `/api/v1/products/{id}` | Consulta por ID |
| POST | `/api/v1/products` | Cadastra produto |
| PUT | `/api/v1/products/{id}` | Atualiza produto |
| DELETE | `/api/v1/products/{id}` | Exclui produto |

Swagger: `http://localhost:5093/swagger` · Saúde: `/health/live` e `/health/ready`.

## Executar localmente

Pré-requisitos: .NET 8 ou superior e SQL Server LocalDB (Visual Studio).

```powershell
dotnet restore "TechStore Cloud.sln"
dotnet build "TechStore Cloud.sln"
dotnet test "TechStoreCloud.Tests/TechStoreCloud.Tests.csproj"
dotnet run --project "TechStore Cloud/TechStore Cloud.csproj" --launch-profile http
```

O frontend Angular está em `../FRONT`. Execute `npm install` e `npm start` nesse diretório; a configuração local já aponta para `http://localhost:5093/api/v1`.

## Azure

- [Arquitetura e decisões](docs/ARQUITETURA.md)
- [Provisionamento com Bicep](infra/README.md)
- [Mapa da rubrica](docs/MAPA_RUBRICA.md)
- [Roteiro de prints do PDF](docs/ROTEIRO_EVIDENCIAS.md)
- [Diagrama com ícones oficiais Azure](docs/architecture/diagrama-arquitetura.svg)

Custos variam por assinatura e região. Confira a página **Free services** e crie um orçamento antes de provisionar. A oferta gratuita do Azure SQL fornece limites mensais e pode pausar a base; a VM gratuita depende da elegibilidade da conta e do SKU/região.
